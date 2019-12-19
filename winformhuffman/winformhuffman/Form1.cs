using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;


namespace winformhuffman
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonCHEMIN_Click(object sender, EventArgs e)
        {  
            openFileDialog1.ShowDialog();
            textBoxCHEMIN.Text = openFileDialog1.FileName;
           
        }

        private void buttonDECOMPRESSER_Click(object sender, EventArgs e)
        {
            char bulle = 'b';
            
            HuffTree compresser = new HuffTree(textBoxCHEMIN.Text, bulle);
            MessageBox.Show("decompression effectuer");
        }

        private void buttonENREIGISTRER_Click(object sender, EventArgs e)
        {
            char bulle = 'a';
            HuffTree compresser = new HuffTree(textBoxCHEMIN.Text, bulle);
            MessageBox.Show("compression effectuer");
        }
    }

    public class HuffNode
    {
        /// <summary>
        /// Obtient/Définit le code ASCII de la feuille(Noeud)
        /// </summary>
        public byte ASCIICode;
        /// <summary>
        /// Obtient/Définit le nombre d'occurence du code ASCII dans le document à compresser
        /// </summary>
        public long Count;
        /// <summary>
        /// Obtient/Définit le Noeud parent (Contient la somme des variables Count des Noeuds Gauche et Droit)
        /// </summary>
        public HuffNode parent;
        /// <summary>
        /// Obtient/Définit le Noeud de gauche (bit = 0)
        /// </summary>
        public HuffNode LNode;
        /// <summary>
        /// Obtient/Définit le Noeud de droite (bit = 1)
        /// </summary>
        public HuffNode RNode;
        /// <summary>
        /// Obtient/Définit le codage huffman de la feuille (="" si ce n'est pas une feuille)
        /// </summary>
        public string CodeWord = "";
        /// <summary>
        /// Constructeur basique d'un noeud sans valeur
        /// </summary>
        public HuffNode()
        {
            this.ASCIICode = 0;
            this.Count = -1;
            this.parent = null;
            this.LNode = null;
            this.RNode = null;
        }
        /// <summary>
        /// Constructeur d'une feuille sans parent
        /// </summary>
        /// <param name="valeur">Code ASCII de la feuille</param>
        /// <param name="occurence">Nombre d'occurence du code ASCII</param>
        public HuffNode(int valeur, long occurence)
        {
            this.ASCIICode = (byte)valeur;
            this.Count = occurence;
            this.parent = null;
            this.LNode = null;
            this.RNode = null;
        }


   
    /// <summary>
    /// Constructeur d'un noeud provenant de la fusion de deux noeuds (aucun code ASCII car pas une feuille)
    /// </summary>
    /// <param name="left">Noeud gauche (bit = 0)</param>
    /// <param name="right">Noeud droit (bit = 1)</param>
    public HuffNode(ref HuffNode left, ref HuffNode right)
        {
            this.parent = null;
            this.ASCIICode = 0;
            this.Count = left.Count + right.Count;
            this.LNode = left;
            this.RNode = right;
        }

        public int HasLRChildren()
        {
            int result = 0;

            if (this.LNode != null)
            {
                result = 1;
            }
            if (this.RNode != null)
            {
                if (result == 1)
                {
                    result++;
                }
                result++;
            }

            return result;
        }
    }

    public class HuffTree : HuffNode
    {
        #region
        /// <summary>
        /// Copie Length octet d'une zone mémoire vers une autre
        /// </summary>
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(
            object Destination,
            object Source,
            int Length);
        #endregion

        public struct HuffWord
        {
            public string Word;
            public int WordSize;
        };
        public int SizeCompressed = 0;
        public int SizeEnco = 0;
        public int SizeTree = 0;
        public byte CRC = 0;
        public short nbNoeud = 0;//Nombre de caractère
        public HuffNode Tree;
        public HuffNode[] tempNode;
        private byte[] toNode;
        public byte[] TheResult;
        public HuffTree.HuffWord[] TheCarWord = new HuffWord[256];
        Stream sian;
        public HuffTree()
        {
            this.parent = null;
            toNode = null;
        }

        public HuffTree(string filePath)
        {
            Stream sin = File.OpenRead(filePath);
            toNode = new byte[sin.Length];
            sin.Read(toNode, 0, (int)sin.Length);
            sin.Close();
            this.parent = null;
        }

        public HuffTree(Stream sin)
        {
            toNode = new byte[sin.Length];
            sin.Read(toNode, 0, (int)sin.Length);
            sian = sin;
            sin.Close();
            this.parent = null;
        }

        public HuffTree(string path, char bulle)
        {
            Stream sin = File.OpenRead(path);
            toNode = new byte[sin.Length];
            sin.Read(toNode, 0, (int)sin.Length);
            sin.Close();
            
            
            if (bulle == 'a')
            {

                byte[] Result = this.Compress(sin);
                Stream sr = File.OpenWrite(path);
                sr.Write(Result, 0, Result.Length);
                sr.Flush();
                sr.Close();
            }


            if (bulle =='b')
            {
                byte[] Result = this.Decompress();
                Stream sr = File.OpenWrite(path);
                sr.Write(Result, 0, Result.Length - 1);
                sr.Flush();
                sr.Close();
            }
           


        }

        public HuffNode[] getFreqKey()
        {
            int i = 0;
            int NodeCount = 0;
            int[] CarCount = new int[256];
            int[] CarValeur = new int[256];

            //Initialisation CarcOunt
            for (i = 0; i < 256; i++)
            {
                CarCount[i] = 0;
            }
            //Compter le nombre d'occurence des caractères
            for (i = 0; i < toNode.Length; i++)
            {
                CarCount[(int)toNode[i]] += 1;
            }
            //Compter le nombre de feuille à créer
            for (i = 0; i < 256; i++)
            {
                if (CarCount[i] > 0)
                {
                    NodeCount += 1;
                }
            }
            //Créer le tableau de feuille
            HuffNode[] Feuille = new HuffNode[NodeCount];
            //Créer les feuilles de chaque code ASCII
            int Debut = 0;
            for (i = 0; i < NodeCount; i++)
            {
                for (int j = Debut; j <= 255; j++)
                {
                    if (CarCount[j] > 0)
                    {
                        Debut = j + 1;
                        Feuille[i] = new HuffNode(j, CarCount[j]);
                        break;
                    }
                }
            }

            return Feuille;
        }

        public HuffNode CreateTree(ref HuffNode[] cNode)
        {
            HuffNode[] tampon = new HuffNode[(2 * cNode.Length) - 1];
            long min1 = 0;
            long min2 = 0;
            int IdLNode = -1;
            int IdRNode = -1;
            int NodeCount = cNode.Length;

            Array.Copy(cNode, tampon, cNode.Length);

            for (int i = NodeCount; i >= 2; i--)
            {
                IdLNode = -1; IdRNode = -1;
                for (int j = 0; j < NodeCount; j++)
                {
                    if (tampon[j].parent == null)
                    {
                        if (IdLNode == -1)
                        {
                            min1 = tampon[j].Count;
                            IdLNode = j;
                        }
                        else if (IdRNode == -1)
                        {
                            min2 = tampon[j].Count;
                            IdRNode = j;
                        }
                        else if (tampon[j].Count < min1)
                        {
                            if (tampon[j].Count < min2)
                            {
                                if (min1 < min2)
                                {
                                    min2 = tampon[j].Count;
                                    IdRNode = j;
                                }
                                else
                                {
                                    min1 = tampon[j].Count;
                                    IdLNode = j;
                                }
                            }
                            else
                            {
                                min1 = tampon[j].Count;
                                IdLNode = j;
                            }
                        }
                        else if (tampon[j].Count < min2)
                        {
                            min2 = tampon[j].Count;
                            IdRNode = j;
                        }
                    }
                }
                tampon[NodeCount] = new HuffNode(ref tampon[IdLNode], ref tampon[IdRNode]);
                tampon[IdLNode].parent = tampon[NodeCount];
                tampon[IdRNode].parent = tampon[NodeCount];
                NodeCount += 1;
            }
            return tampon[NodeCount - 1];
        }

        public void CreateHuffSeq(ref HuffNode curNode, ref HuffWord[] CarWord, ref short nbNoeud)
        {
            int curNChild = curNode.HasLRChildren();

            if (curNChild == 1)//Un noeud gauche
            {
                curNode.LNode.CodeWord += (curNode.parent != null ? curNode.CodeWord : "") + "0";
                CreateHuffSeq(ref curNode.LNode, ref CarWord, ref nbNoeud);
            }
            else if (curNChild == 2)//Un noeud droit
            {
                curNode.RNode.CodeWord += (curNode.parent != null ? curNode.CodeWord : "") + "1";
                CreateHuffSeq(ref curNode.RNode, ref CarWord, ref nbNoeud);
            }
            else if (curNChild == 3)//Deux noeud gauche et droit
            {
                curNode.LNode.CodeWord += (curNode.parent != null ? curNode.CodeWord : "") + "0";
                CreateHuffSeq(ref curNode.LNode, ref CarWord, ref nbNoeud);
                curNode.RNode.CodeWord += (curNode.parent != null ? curNode.CodeWord : "") + "1";
                CreateHuffSeq(ref curNode.RNode, ref CarWord, ref nbNoeud);
            }
            else//Pas de noeud
            {
                //On créer le mot huffman pour ce caractère
                CarWord[curNode.ASCIICode].Word = curNode.CodeWord;
                CarWord[curNode.ASCIICode].WordSize = curNode.CodeWord.Length;
                tempNode[(int)--nbNoeud] = curNode;
            }
        }

        public void ClassHuffNodes()
        {
            HuffNode[] tampon = new HuffNode[tempNode.Length];
            int curTampon = 0;
            for (int i = 0; i <= 255; i++)
            {
                for (int j = 0; j < tempNode.Length; j++)
                {
                    if (tempNode[j].ASCIICode == (byte)i)
                    {
                        tampon[curTampon] = tempNode[j];
                        curTampon++;
                        break;
                    }
                }
            }
            tempNode = tampon;
        }
        public void CalculCRC()
        {
            for (int i = 0; i < toNode.Length; i++)
            {
                CRC = (byte)(CRC ^ toNode[i]);
            }
        }

        public int CalculTotalSize()
        {
            //Calcul le mombre de bit et renvoit le nombre d'octet du fichier encodé
            //utilisation du type long pour obtenir des divisions entières
            SizeCompressed = 0;
            SizeTree = 0;
            SizeEnco = 0;

            SizeCompressed = 40;//Taille descriptif encodage (byte = 8bit)*4=32 + taille CRC (byte=8bit)*1=8 (en bit 32+8=40) 
            SizeCompressed += 32;//Taille du fichier source (intC# = 32bit)*1
            SizeCompressed += 16;//Nombre de caractère utilisé (short=16bit)*1
            for (int i = 0; i < tempNode.Length; i++)
            {
                SizeCompressed += 8;//Le caractère du noeud courant (byte=8bit)*1
                SizeCompressed += 8;//Taille du code Caractère
                SizeEnco += (int)(tempNode[i].CodeWord.Length * tempNode[i].Count);//Taille de la source encodé (en bit)
                SizeTree += (int)tempNode[i].CodeWord.Length; //Taille de l'arbre (en bit)
            }
            SizeTree = (int)(SizeTree % 8 == 0 ? SizeTree / 8 : (SizeTree / 8) + 1);//Complément du manque de bit pour faire un compte juste d'octet
            SizeEnco = (int)(SizeEnco % 8 == 0 ? SizeEnco / 8 : (SizeEnco / 8) + 1);//Complément du manque de bit pour faire un compte juste d'octet

            SizeCompressed = (SizeCompressed % 8 == 0 ? SizeCompressed / 8 : (SizeCompressed / 8) + 1) + SizeTree + SizeEnco;

            return SizeCompressed;
        }

        public byte[] Decompress()
        {
            bool decomp = false;
            int curByte = 0;
            byte[] fileUncompress = new byte[0];

            if (toNode[0] == 'H' && toNode[1] == 'E' && toNode[2] == '3' && toNode[3] == 13)
            {
                decomp = true;//Flux compressé à décompresser
            }
            else if (toNode[0] == 'H' && toNode[1] == 'E' && toNode[2] == '0' && toNode[3] == 13)
            {
                fileUncompress = new byte[] { (byte)'N', (byte)'C' };
            }
            else
            {
                fileUncompress = new byte[] { (byte)'M', (byte)'F' };
            }

            if (decomp)
            {
                this.CRC = toNode[4];
                SizeCompressed = Marshal.ReadInt32(toNode, 5);
                nbNoeud = Marshal.ReadInt16(toNode, 9);
                HuffNode[] cNode = new HuffNode[nbNoeud];
                curByte = 11;
                int BitPos = 0;

                for (int i = 0; i < nbNoeud; i++)
                {
                    cNode[i] = new HuffNode();
                    cNode[i].ASCIICode = toNode[curByte++];
                    cNode[i].CodeWord = new string('0', (int)toNode[curByte++]);
                }
                byte TheByte = toNode[curByte++];
                for (int i = 0; i < cNode.Length; i++)
                {
                    int lngCode = cNode[i].CodeWord.Length;
                    for (int j = 0; j < lngCode; j++)
                    {
                        byte puiss2 = (byte)Math.Pow((double)2, (double)BitPos);
                        if ((TheByte & puiss2) > 0)
                        {
                            cNode[i].CodeWord += "1";
                        }
                        else
                        {
                            cNode[i].CodeWord += "0";
                        }
                        BitPos++;
                        if (BitPos == 8)
                        {
                            TheByte = toNode[curByte++];
                            BitPos = 0;
                        }
                    }
                    cNode[i].CodeWord = cNode[i].CodeWord.Remove(0, lngCode);
                }
                if (BitPos > 0)
                {
                    curByte++;
                }
                tempNode = cNode;
                //Tree = CreateTree(ref tempNode);
                fileUncompress = new Byte[SizeCompressed];
                Tree = new HuffNode(-1, 0);
                Tree.parent = null;
                HuffNode curNode = Tree;
                int ResultLen = 0;
                //Recréation de l'arbre
                for (int i = 0; i < cNode.Length; i++)
                {
                    curNode = Tree;
                    for (int j = 0; j < cNode[i].CodeWord.Length; j++)
                    {
                        if (cNode[i].CodeWord.Substring(j, 1).Equals("0") && j < cNode[i].CodeWord.Length - 1)
                        {
                            if (curNode.LNode == null)
                            {
                                curNode.LNode = new HuffNode(-1, 0);
                                curNode.LNode.parent = curNode;
                            }
                            curNode = curNode.LNode;
                        }
                        else if (cNode[i].CodeWord.Substring(j, 1).Equals("1") && j < cNode[i].CodeWord.Length - 1)
                        {
                            if (curNode.RNode == null)
                            {
                                curNode.RNode = new HuffNode(-1, 0);
                                curNode.RNode.parent = curNode;
                            }
                            curNode = curNode.RNode;
                        }
                        else if (cNode[i].CodeWord.Substring(j, 1).Equals("0") && j == cNode[i].CodeWord.Length - 1)
                        {
                            curNode.LNode = cNode[i];
                            curNode.LNode.parent = curNode;
                            //Tree = curNode;
                        }
                        else if (cNode[i].CodeWord.Substring(j, 1).Equals("1") && j == cNode[i].CodeWord.Length - 1)
                        {
                            curNode.RNode = cNode[i];
                            curNode.RNode.parent = curNode;
                            //Tree = curNode;
                        }
                    }
                }

                //Decode les informations
                curNode = Tree;
                for (int i = curByte - 1; i < toNode.Length; i++)
                {
                    TheByte = toNode[i];
                    for (int j = 0; j < 8; j++)
                    {
                        byte puiss2 = (byte)Math.Pow((double)2, (double)j);
                        if ((TheByte & puiss2) > 0)
                        {
                            if (curNode.RNode.LNode == null && curNode.RNode.RNode == null)
                            {
                                fileUncompress[ResultLen++] = curNode.RNode.ASCIICode;
                                curNode = Tree;
                            }
                            else
                            {
                                curNode = curNode.RNode;
                            }
                        }
                        else
                        {
                            if (curNode.LNode.LNode == null && curNode.LNode.RNode == null)
                            {
                                fileUncompress[ResultLen++] = curNode.LNode.ASCIICode;
                                curNode = Tree;
                            }
                            else
                            {
                                curNode = curNode.LNode;
                            }
                        }
                        if (ResultLen == fileUncompress.Length)
                        {
                            break;
                        }
                    }
                    if (ResultLen == fileUncompress.Length)
                    {
                        break;
                    }
                }
                //Test si le fichier est corrompue
                byte finalCRC = 0;
                for (int i = 0; i < fileUncompress.Length; i++)
                {
                    finalCRC = (byte)(finalCRC ^ fileUncompress[i]);
                }

                if (!(CRC == finalCRC))
                {
                    fileUncompress = new byte[] { (byte)'C', (byte)'R' };
                }

            }
            return fileUncompress;
        }

        public byte[] Compress(Stream sin)
        {
            byte[] fileCompressed;
            int curByte = 0;
            
            CalculCRC();
            tempNode = getFreqKey();
            nbNoeud = (short)tempNode.Length;
            Tree = CreateTree(ref tempNode);
            tempNode = new HuffNode[nbNoeud];
            CreateHuffSeq(ref Tree, ref TheCarWord, ref nbNoeud);
            nbNoeud = (short)tempNode.Length;
            SizeCompressed = CalculTotalSize();
            ClassHuffNodes();
            if (SizeEnco >= toNode.Length)
            {
                //Retourne un fichier non compressé
                fileCompressed = new byte[toNode.Length + 4]; //Taille du fichier + en-tête HE0['\13']
                fileCompressed[0] = (byte)'H';
                fileCompressed[1] = (byte)'E';
                fileCompressed[2] = (byte)'0';
                fileCompressed[3] = (byte)13;
                //CopyMemory(fileCompressed[4],toNode[0],toNode.Length);
                Array.Copy(toNode, 0, fileCompressed, 4, toNode.Length);
            }
            else
            {
                fileCompressed = new byte[SizeCompressed];
                for (int i = 0; i < SizeCompressed; i++)
                {
                    fileCompressed[i] = 0;
                }
                fileCompressed[0] = (byte)'H';
                fileCompressed[1] = (byte)'E';
                fileCompressed[2] = (byte)'3';
                fileCompressed[3] = (byte)13;
                //Ajoute le CRC
                fileCompressed[4] = CRC;
                curByte = 5;
                //Ajoute la taille du fichier source
                Marshal.WriteIntPtr(fileCompressed, curByte, (IntPtr)toNode.Length);
                curByte += 4;
                //Ajoute le nb de caractère utilisé
                Marshal.WriteIntPtr(fileCompressed, curByte, (IntPtr)nbNoeud);
                //Ajoute le descriptif de l'arbre d'huffman
                curByte += 2;
                for (int i = 0; i < (int)nbNoeud; i++)
                {
                    fileCompressed[curByte++] = tempNode[i].ASCIICode;
                    fileCompressed[curByte++] = (byte)tempNode[i].CodeWord.Length;
                }
                int bitPos = 0;
                byte ByteValue = 0;
                for (int i = 0; i < tempNode.Length; i++)
                {
                    for (int j = 0; j < tempNode[i].CodeWord.Length; j++)
                    {
                        if (tempNode[i].CodeWord.Substring(j, 1).Equals("1"))
                        {
                            ByteValue += (byte)System.Math.Pow((double)2, (double)bitPos);
                        }
                        bitPos++;
                        if (bitPos == 8)
                        {
                            fileCompressed[curByte++] = ByteValue;
                            bitPos = 0;
                            ByteValue = 0;
                        }
                    }
                }
                if (bitPos > 0)
                {
                    fileCompressed[curByte++] = ByteValue;
                }
                bitPos = 0;
                ByteValue = 0;
                //Ajoute la source encodée
                for (int i = 0; i < toNode.Length; i++)
                {
                    string curHuffCod = getCodeWordOf(toNode[i]);
                    for (int j = 0; j < curHuffCod.Length; j++)
                    {
                        if (curHuffCod.Substring(j, 1).Equals("1"))
                        {
                            ByteValue += (byte)System.Math.Pow((double)2, (double)bitPos);
                        }
                        bitPos++;
                        if (bitPos == 8)
                        {
                            fileCompressed[curByte++] = ByteValue;
                            bitPos = 0;
                            ByteValue = 0;
                        }
                    }
                }
                if (bitPos > 0)
                {
                    fileCompressed[curByte++] = ByteValue;
                }
            }

            return fileCompressed;
        }
      
        public string getCodeWordOf(byte car)
        {
            string result = "";

            for (int i = 0; i < tempNode.Length; i++)
            {
                if (tempNode[i].ASCIICode == car)
                {
                    result = tempNode[i].CodeWord;
                    break;
                }
            }
            return result;
        }
    }
}
