using System;

namespace CompareXML
{
    /// <summary>
    /// DTO représentant un support comosant
    /// </summary>
    public class SupportComposant : IEquatable<SupportComposant>
    {
        /// <summary>
        /// Identifiant du support composant
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Identifiant du support info générale
        /// </summary>
        public string IDInfoGenerale { get; set; }
        /// <summary>
        /// Valeur de la cotation
        /// </summary>
        public string ValeurCotation { get; set; }
        /// <summary>
        /// Nombre de parts
        /// </summary>
        public string NbParts { get; set; }
        /// <summary>
        /// Date de la cotation
        /// </summary>
        public string DateCotation { get; set; }
        /// <summary>
        /// Date de valorisation
        /// </summary>
        public string DateValorisation { get; set; }
        /// <summary>
        /// Montant (en euros)
        /// </summary>
        public string Montant { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdContrat { get; set; } //Liaison avec le contrat parent

        //public bool Equals(SupportComposant other) => ID == other.ID ? true : false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SupportComposant other) => IDInfoGenerale == other.IDInfoGenerale ? true : false;

        //public override string ToString() => $"[Contrat_ID : {IdContrat}] -- ID : {ID}";
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"[Contrat_ID : {IdContrat}] -- ID : {ID} - ID_Info_Generale : {IDInfoGenerale}";
    }
}
