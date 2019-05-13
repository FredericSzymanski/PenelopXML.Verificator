using System;

namespace CompareXML
{
    /// <summary>
    /// DTO représentant un intermédiaire
    /// </summary>
    public class IntermediaireConsomme : IEquatable<IntermediaireConsomme>
    {
        /// <summary>
        /// Identifiant de l'intermédiaire
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Code interne de l'intermédiaire
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Nom de l'intermédiaire
        /// </summary>
        public string Denomination { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IntermediaireConsomme other) => Id == other.Id ? true : false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"ID : {Id} - code interne : {Code} - denomination : {Denomination}";
    }
}
