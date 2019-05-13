using System;

namespace CompareXML
{
    /// <summary>
    /// DTO représentant un epersonne physique ou morale
    /// </summary>
    public class Personne: IEquatable<Personne>
    {
        /// <summary>
        /// Identifiant de la personne
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Nom de la personne 
        /// </summary>
        public string Nom { get; set; }
        /// <summary>
        /// prénoms de la personne physique
        /// </summary>
        public string Prenoms { get; set; }
        /// <summary>
        /// Civilté de la personne physique
        /// </summary>
        public string Civilite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Personne other) => ID == other.ID ? true : false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"ID : {ID}\tNom : {Nom}\tPrénom : {Prenoms}\tCivilité : {Civilite}";
    }
}
