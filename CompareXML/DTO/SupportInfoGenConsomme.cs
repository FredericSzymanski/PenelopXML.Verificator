using System;

namespace CompareXML
{
    /// <summary>
    /// DTO représentant un support info générale
    /// </summary>
    public class SupportInfoGenConsomme : IEquatable<SupportInfoGenConsomme>
    {
        /// <summary>
        /// Identifiant du support info générale
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Origine du code courant
        /// </summary>
        public string OrigineCodeCourant { get; set; }
        /// <summary>
        /// Code courant
        /// </summary>
        public string CodeCourant { get; set; }
        /// <summary>
        /// Origine du code précedent
        /// </summary>
        public string OrigineCodePrecedent { get; set; }
        /// <summary>
        /// Code précedent
        /// </summary>
        public string CodePrecedent { get; set; }
        /// <summary>
        /// Désignation du support info générale
        /// </summary>
        public string  Designation { get; set; }
        /// <summary>
        /// Identifiant du fournisseur associé
        /// </summary>
        public string IdFournisseur { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SupportInfoGenConsomme other) => Id == other.Id ? true : false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"ID : {Id} - designation : {Designation} - ref ID fournisseur : {IdFournisseur}";
    }
}
