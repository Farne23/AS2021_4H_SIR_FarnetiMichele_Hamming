using System;
using System.Collections.Generic;

namespace AS2021_4H_SIR_FarnetiMichele_Hamming
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Calcolo della codifica di Hamming di Michele Farneti\r\n");

            string input1;
            string input2;

            do
            {
                Console.WriteLine("Inserire il valore binario: ");
                input1 = Console.ReadLine();

                if (ItIsBinary(input1) == true)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Valore inserito non valido, inserire una stringa di 0 e 1");
                }
            }
            while (true);

            Console.WriteLine($"Codifica di Hamming della parola in ingresso::");
            StampaHamming(ParitaDiHamming(input1));
            Console.WriteLine();

            do
            {
                Console.WriteLine("\r\nInserisci la parola ricevuta (comprensiva di codice di Hamming), inserendovi al massimo un errore: ");
                input2 = Console.ReadLine();

                if (ItIsBinary(input2) == true)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Valore inserito non valido, inserire una stringa di 0 e 1");
                }      
            } while (true);

            if(ParitaDiHamming(input1) == input2)
            {
                Console.WriteLine("\r\n Ricezione avvenuta con successo, nessun errore da notificare");
            }
            else if (CorrezioneHammingApplicabile(ParitaDiHamming(input1), input2)==true)
            {
                int PosizioneErrore = PosizioneErroreHamming(input2);

                Console.WriteLine("\r\nRicostruzione della parola corretta:");
                input2 = CorreggiErrore(input2, PosizioneErrore);
                StampaHamming(input2, PosizioneErrore);

                Console.WriteLine($"\r\nErrore rilevato e corretto al bit {PosizioneErrore}");
            }
            else
            {
                Console.WriteLine("\r\nCorrezione non applicabile, sono presenti piú di un \r\nerrore o la lunghezza del valore binario é errata");
            }
        }

        //Funzione per verificare se una stinga sia o meno un valore binario
        public static bool ItIsBinary(string valore)
        {
            bool Corretto = true;

            foreach(char element in  valore.ToCharArray())
            {
                if(element!= '1' && element != '0')
                {
                    Corretto = false;
                    break;
                }
            }

            return (Corretto);

        }

        //Calcolo della parita di hamming, restituisce una stringa con Hamming aplicato
        public static string ParitaDiHamming(string valorebinario)
        {
            char[] valorebinarioChar = valorebinario.ToCharArray();
            int lunghezzainput = valorebinario.Length; 
            int nSlotParita=0;


            //Conto il numero di bit aggiuntivi da dedicare alle paritá
            for(int i =0; i< lunghezzainput; i++)
            {
                if(Math.Pow(2, i) > lunghezzainput)
                {
                    nSlotParita = i;
                    break;           
                }
                lunghezzainput++;
            }

            //Genero un nuovo array della distanza corretta
            int lunghezzaoutput = valorebinario.Length + nSlotParita;
            char[] risultatoChar = new char[lunghezzaoutput];

            //Utilizzo dei caratteri '_' per occupare le posizioni che verrano dedicate alle parita
            for(int j = 0; j<lunghezzaoutput; j++)
            {
                if(Math.Pow(2, j) < lunghezzaoutput)
                {
                    risultatoChar[Convert.ToInt32((Math.Pow(2, j)-1))] = '_';
                }
            }

            //Valorizzo le posizioni della matrice non occupate dalle paritá
            int y = 0;
            for (int j = 0; j < lunghezzaoutput; j++)
            {
                if (risultatoChar[j]!='_')
                {
                    risultatoChar[j] = valorebinarioChar[y];
                    y++;
                }
            }

            //Calcolo le varie pairtá
            string Parita;

            //Primo ciclo, ogni giro corrisponde ad una paritá da calolare
            for (int i = 0; i< nSlotParita; i++)
            {

                //Secondo ciclo, avanza con passi dal valore crescente per ogni paritá (1,2,4,8 ecc)
                Parita = "";
                for (int j = Convert.ToInt32(Math.Pow(2,i))-1; j < lunghezzaoutput; j += Convert.ToInt32(Math.Pow(2, i)))
                {
                    //Terzo ciclo, una volta eseguito il primo salto vengono salvati i
                    //valori situati nella posizione e nelle succesive (dunque 2^i slot)
                    int intervallo = j + Convert.ToInt32(Math.Pow(2, i));
                    do
                    {
                        //Qualora la posizione non fosse occupata da una paritá, il
                        //dato viene salvato in una stringa e sommato agli altri
                        if (risultatoChar[j] != '_')
                        {
                            Parita += risultatoChar[j];
                        }
                        j++;
                    } 
                    while (j < intervallo && j<lunghezzaoutput);
                }

                //Individuati i valori di cui sia necessario calcolare la paritá, viene richiamta
                //la funzione per esegueire il calcolo ed il risultato viene inserito nello slot
                //corrispondente
                risultatoChar[Convert.ToInt32(Math.Pow(2, i)) - 1] = ($"{BitDiParitaPari(Parita)}".ToCharArray())[0]; 
            }

            //Restituisco la nuova string con applicato hamming
            string risultato = new String(risultatoChar);         
            return (risultato);
        }

        //Funzione per il calcolo del bit di parita pari, l'argometno che viene passato é una stringa
        public static int BitDiParitaPari(string valore)
        {
            //Trasformo la stringa in un array di caratteri 
            char[] valoreChar = valore.ToCharArray();
            //Inizializzo un contatore per gli '1' presenti
            int counter = 0;

            //Eseguo un ciclo for per studiare ogni carattere della stringa
            for (int i = 0; i < valoreChar.Length; i++)
            {
                //Qualora il carattere fosse uguale a '1' incremento il contatore
                if (valoreChar[i] == '1')
                {
                    counter++;
                }

                //Qualora la stringa contenesse caratteri diversi da 1 e 0 restituirei
                //un un numero per segnalare l'errore ossia -1
                if (valoreChar[i] != '0' && valoreChar[i] != '1')
                {
                    return (-1);
                }
            }

            //Verifico il risultato del counter e sei il numero di bit a 1 é pari 
            //restituisco 0, altrimenti restitusco 1, l'output é sotto forma di int 
            if (counter % 2 == 0)
            {
                return (0);
            }
            else
            {
                return (1);
            }
        }

        //Funzione per la stampa di un numero con applicato hamming evidenziandone le parità
        public static void StampaHamming(string valore)
        {
            char[] valoreChar = valore.ToCharArray();

            int j = 0;
            for(int i = 0; i<valoreChar.Length; i++)
            {
                if (Math.Pow(2, j) - 1 == i)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(valoreChar[i]);
                    Console.ResetColor();
                    j++;
                }
                else
                {
                    Console.Write(valoreChar[i]);
                }
            }
        }

        //Funzione per la stampa di un numero con applicato hamming evidenziandone le parità ed un bit di errore
        public static void StampaHamming(string valore, int errore)
        {
            char[] valoreChar = valore.ToCharArray();

            int j = 0;
            for (int i = 0; i < valoreChar.Length; i++)
            {
                if (Math.Pow(2, j) - 1 == i)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    if(i + 1 == errore)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    Console.Write(valoreChar[i]);
                    Console.ResetColor();
                    j++;
                }
                else
                {
                    if (i + 1 == errore)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }

                    Console.Write(valoreChar[i]);
                    Console.ResetColor();           
                }
            }
            Console.WriteLine();
        }

        //Funzione che restituisce la posizione di un errore (partendo da 1) dato un 
        //valore binario con Hamming applicato
        public static int PosizioneErroreHamming(string valorebinario)
        {
            char[] valore = valorebinario.ToCharArray();

            int posizione=0;
            string Parita;

            //Controllo le varie parita (p1,p2,p4,p8) tenendo conto anche dei parita 
            for (int i = 0; i < 3; i++)
            {
                Parita = "";

                for (int y = Convert.ToInt32(Math.Pow(2, i)) - 1; y < valore.Length; y += Convert.ToInt32(Math.Pow(2, i)))
                {                    
                    int intervallo = y + Convert.ToInt32(Math.Pow(2, i));
                    do
                    {
                        Parita += valore[y];
                        y++;
                    }
                    while (y < intervallo && y < valore.Length);               
                }
                //Il bit di parita delle p1,p2,p4 ecc deve dare come risultato zero per via delle paritá di hamming che sono incluse
                //In caso contrario si procede con il calcolo della posizione dell'errore
                if (BitDiParitaPari(Parita) != 0) 
                { 
                    posizione += (Convert.ToInt32(Math.Pow(2,i))); 
                }            
            }
            return (posizione);
        }

        //Corregge il bit n di un valore binario
        public static string CorreggiErrore(string valorebinario,  int posizione)
        {
            char[] valore = valorebinario.ToCharArray();

            //Risalgo alla posizione dell'errore ed inverto il valore del bit
            if (valore[posizione - 1] == '1')
            {
                valore[posizione - 1] = '0';
            }
            else if (valore[posizione - 1] == '0')
            {
                valore[posizione - 1] = '1';
            }

            string risultato = new String(valore);
            return (risultato);
        }

        //Verifica se é possibile corregere un errore, cioé se vi é massimo un errore tra due valori binari con hamming apllicato
        public static bool CorrezioneHammingApplicabile(string valore1, string valore2)
        {
            if (valore1.Length != valore2.Length)
            {
                return (false);
            }

            char[] valore1Char = valore1.ToCharArray();
            char[] valore2Char = valore2.ToCharArray();

            int contatore=0;

            for(int i = 0; i< valore1Char.Length; i++)
            {
                if (valore1Char[i] != valore2Char[i])
                {
                    contatore++;
                }
            }

            if (contatore == 1)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }
    }
}
