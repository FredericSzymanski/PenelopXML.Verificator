using System;

namespace CompareXML
{
    /// <summary>
    /// DTO représentant un fournisseur
    /// </summary>
    public class FournisseurConsomme : IEquatable<FournisseurConsomme>
    {
        /// <summary>
        /// Identifiant du fournisseur
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Nom du fournisseur
        /// </summary>
        public string Libelle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FournisseurConsomme other) => Id == other.Id ? true : false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"ID : {Id} - Libelle court : {Libelle}";
    }
}
