using System;

namespace CompareXML
{
    /// <summary>
    /// DTO représentant un produit financier
    /// </summary>
    public class Produit : IEquatable<Produit>
    {
        /// <summary>
        /// Identifiant du produit
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Identifiant du fournisseurn associé au produit
        /// </summary>
        public string RefFournisseur { get; set; }
        /// <summary>
        /// Code type de produit
        /// </summary>
        public string CodeTypeProduit { get; set; }
        /// <summary>
        /// Code régime fiscal
        /// </summary>
        public string CodeRegimeFiscal { get; set; }
        /// <summary>
        /// Nom du produit
        /// </summary>
        public string Designation { get; set; }
        /// <summary>
        /// Flag multisupport (OUI ou NON)
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Produit other) => Id == other.Id ? true : false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"ID : {Id} - designation : {Designation}";
    }
}
