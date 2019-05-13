using System;
using System.Collections.Generic;

namespace CompareXML
{
    /// <summary>
    /// DTO représentant un contrat
    /// </summary>
    public class Contrat: IEquatable<Contrat>
    {
        /// <summary>
        /// ID Penelop du contrat
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// N° de contrat
        /// </summary>
        public string Numero { get; set; }
        /// <summary>
        /// Identifiant de la référence du produit asocié
        /// </summary>
        public string RefProduit { get; set; }
        /// <summary>
        /// Identofiant du conseiller lié au contrat
        /// </summary>
        public string refConseiller { get; set; }
        /// <summary>
        /// Code régime fiscal
        /// </summary>
        public string RegimeFiscal { get; set; }
        /// <summary>
        /// Date de la situation
        /// </summary>
        public string DateSituation { get; set; }
        /// <summary>
        /// Date d'effet du contrat
        /// </summary>
        public string DateEffet { get; set; }
        /// <summary>
        /// Dode état
        /// </summary>
        public string CodeEtat { get; set; }
        /// <summary>
        /// Liste de rôles liés au contrat
        /// </summary>
        public List<Role> Roles { get; set; }
        /// <summary>
        /// Lsite des Support_Composants liés au contrat
        /// </summary>
        public List<SupportComposant> SupportComposants { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Contrat other) => ID == other.ID ? true : false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"ID : {ID}\tNuméro : {Numero}\tRefProduit : {RefProduit}\tRefConseiller : {refConseiller}\tRegimeFiscal : {RegimeFiscal}\nDate' situation : {DateSituation}\tDate effet : {DateEffet}\tCode état : {CodeEtat}";
    }
}
