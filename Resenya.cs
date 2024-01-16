using System.IO;

namespace ResenyasAvalonia
{
    internal class Resenya
    {
        public string Titulo { get; set; }
        public double Nota { get; set; }
        public int Anyo {  get; set; }
        public string Contenido {  get; set; }
        public bool Recomendado { get; set; }
        public string Generos {  get; set; }
        public byte[] Imagen { get; set; }
        public char Tipo {  get; set; }

        public Resenya(
            char tipo, 
            string titulo,
            int anyo, 
            string generos,
            double nota,
            string contenido,
            bool recomendado,
            byte[] imagen
        ) 
        { 
            this.Tipo = tipo;
            this.Titulo = titulo;
            this.Anyo = anyo;
            this.Generos = generos;
            this.Nota = nota;
            this.Contenido = contenido;
            this.Recomendado = recomendado;
            this.Imagen = imagen;
        }

        public static Resenya cargarResenya(BinaryReader br)
        {
            char tipo = br.ReadChar();
            string titulo = br.ReadString();
            int anyo = br.ReadInt32();
            string generos = br.ReadString();
            double nota = br.ReadDouble();
            string contenido = br.ReadString();
            bool recomendado = br.ReadBoolean();
            int tam = br.ReadInt32();
            byte[] arrImage = new byte[tam];
            for (int i = 0; i < tam; i++)
            {
                arrImage[i] = br.ReadByte();
            }

            return new Resenya(
                tipo,
                titulo,
                anyo, 
                generos,
                nota,
                contenido, 
                recomendado,
                arrImage
            );
        }

        public void guardarResenya(BinaryWriter bw)
        {
            bw.Write(Tipo);
            bw.Write(Titulo);
            bw.Write(Anyo);
            bw.Write(Generos);
            bw.Write(Nota);
            bw.Write(Contenido);
            bw.Write(Recomendado);
            bw.Write(Imagen.Length);
            bw.Write(Imagen);
        }

    }
}
