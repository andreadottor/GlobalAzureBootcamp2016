using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.Azure.WebJobs;

namespace Dottor.gab16pn.BlobImageResizeWebJob
{
    public class Functions
    {
        /// <summary>
        /// Funzione che ridimensione un'immagine quando viene caricata in un blob
        /// </summary>
        public static void Create640x([BlobTrigger("gab16pn-input/{name}")] Stream input,
                              [Blob("gab16pn/640x/{name}", FileAccess.Write)] Stream output)
        {
            Resize(640, input, output);
        }

        /// <summary>
        /// Funzione che ridimensione un'immagine quando viene caricata in un blob
        /// </summary>
        public static void Create240x([BlobTrigger("gab16pn-input/{name}")] Stream input,
                                      [Blob("gab16pn/240x/{name}", FileAccess.Write)] Stream output)
        {
            Resize(240, input, output);
        }


        private static void Resize(int width, Stream input, Stream output)
        {
            try
            {
                using (var factory = new ImageFactory(preserveExifData: true))
                using (var memory = new MemoryStream())
                {
                    factory.Load(input)
                      .Resize(new Size(width, 0))
                      .Format(new JpegFormat { Quality = 75 })
                      .Save(memory);

                    memory.CopyTo(output);
                }
            }
            catch (Exception ex)
            {

                Console.Error.Write(ex.ToString());
            }
            
        }
    }
}
