using System;

namespace CompareXML
{
    /// <summary>
    /// DTO représentant un rôle
    /// </summary>
    public class Role : IEquatable<Role>
    {
        /// <summary>
        /// Identification du rôle
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Identifiant de la personne associée
        /// </summary>
        public string IdPersonne { get; set; }
        /// <summary>
        /// Code rôle
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Identifiant du contrat parent
        /// </summary>
        public string IdContrat { get; set; } //Liaison avec le contrat parent

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Role other) => ID == other.ID ? true : false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"[Contrat_ID : {IdContrat}] -- ID : {ID}";
    }
}
