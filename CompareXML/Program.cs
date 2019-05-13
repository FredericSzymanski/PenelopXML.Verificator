using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CompareXML
{
    class Program
    {
        #region Pour tests
        private const string OldFileName = @"XML\reference_20190416_Penelop_C2I.xml";
        private const string NewFileName = @"XML\20190416_C2I_Penelop.xml";
        #endregion

        //Fichier LOG
        private const string LogFile = "Comparaison_XML_Penelop.log";
        //Fichiers XML à comparer
        private static XDocument oldDoc;
        private static XDocument newDoc;
        //Nom des fichier XML à comparer (pour LOG)
        private static string oldDocName, newDocName;
        //Listes statiques utiles pour l'ananlyse des contrats
        private static List<SupportComposant> supportComposantsManquants = new List<SupportComposant>();
        private static List<Role> rolesManquants = new List<Role>();


        /// <param name="oldfile">Path du fichier de référence (OLD)</param>
        /// <param name="newfile">Path du fichier Penelop V3</param>
        static void Main(string oldfile = OldFileName, string newfile = NewFileName)
        {
            try
            {
                #region Initialisation des logs

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File(LogFile, restrictedToMinimumLevel: LogEventLevel.Information)
                    .Enrich.WithProperty("Application", "XML Penelop")
                    .CreateLogger();

                #region Version avec args (commentée)
                ////SetCommandMLineOptions(args);

                //if (args.Count() == 0) // Pour tests
                //{
                //    oldDocName = OldFileName;
                //    newDocName = NewFileName;
                //    oldDoc = XDocument.Load(OldFileName);
                //    newDoc = XDocument.Load(NewFileName);
                //}
                //else if (args.Count() == 2)
                //{
                //    oldDocName = args[0];
                //    newDocName = args[1];
                //    oldDoc = XDocument.Load(oldDocName);
                //    newDoc = XDocument.Load(newDocName);
                //}
                //else
                //{
                //    Log.Debug("ARGUMENTS INVALIDES");
                //    Log.CloseAndFlush();
                //    return;
                //}
                #endregion

                oldDocName = oldfile;
                newDocName = newfile;
                oldDoc = XDocument.Load(oldDocName);
                newDoc = XDocument.Load(newDocName);
                #endregion

                Traitement();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        #region Ligne de commande (OBSOLETE)
        /// <summary>
        /// Mise en place de la ligne de commande
        /// </summary>
        /// <param name="args">Arguments du Main</param>
        /// <returns></returns>
        [Obsolete]
        private static int SetCommandMLineOptions(string[] args)
        {
            Option optFileInfoOld = new Option(
                    new[] { "--oldfile", "-of" },
                    "Path du fichier de référence (OLD)",
                    new Argument<FileInfo>(defaultValue: new FileInfo(OldFileName)));

            Option optFileInfoNew = new Option(
                new[] { "--newfile", "-nf" },
                "Path du fichier Penelop V3 (NEW)",
                new Argument<FileInfo>(defaultValue: new FileInfo(NewFileName)));

            var rootCommand = new RootCommand();
            rootCommand.Description = "Outil de comparaison de fichiers XML pour le développement de Penelop V3";
            rootCommand.AddOption(optFileInfoOld);
            rootCommand.AddOption(optFileInfoNew);

            rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo>(
                (oldfile, newfile) =>
                    {
                        Console.WriteLine($"La valeur pour l'option --oldfile est : {oldfile?.Name ?? "null"}");
                        Console.WriteLine($"La valeur pour l'option --newfile est : {newfile?.Name ?? "null"}");
                    });

            return rootCommand.InvokeAsync(args).Result;
        }
        #endregion

        private static void Traitement()
        {
            #region Début de log
            var guid = Guid.NewGuid();
            Log.Information($"DEBUT LOG {guid}");
            Log.Debug(String.Empty);
            Log.Information($"VALEUR DU OLD : {oldDocName}");
            Log.Information($"VALEUR DU NEW : {newDocName}");
            Log.Debug(String.Empty);
            #endregion

            #region Récupération des données XML
            List<Personne> personnesOld = new List<Personne>();
            List<Personne> personnesNew = new List<Personne>();

            RetrievePersonnes(personnesOld, personnesNew);

            Log.Information($"Nombre de personnes dans le OLD : {personnesOld.Count()}");
            Log.Information($"Nombre de personnes dans le NEW : {personnesNew.Count()}");
            Log.Debug(String.Empty);

            List<Contrat> contratsOld = new List<Contrat>();
            List<Contrat> contratsNew = new List<Contrat>();

            RetrieveContracts(contratsOld, contratsNew);

            Log.Information($"Nombre de contrats dans le OLD : {contratsOld.Count()}");
            Log.Information($"Nombre de contrats dans le NEW : {contratsNew.Count()}");
            Log.Debug(String.Empty);

            List<FournisseurConsomme> fournisseursOld = new List<FournisseurConsomme>();
            List<FournisseurConsomme> fournisseursNew = new List<FournisseurConsomme>();
            RetrieveFournisseurs(fournisseursOld, fournisseursNew);

            Log.Information($"Nombre de fournisseurs consommes dans le OLD : {fournisseursOld.Count()}");
            Log.Information($"Nombre de fournisseurs consommes dans le NEW : {fournisseursNew.Count()}");
            Log.Debug(String.Empty);

            List<IntermediaireConsomme> intermediairesOld = new List<IntermediaireConsomme>();
            List<IntermediaireConsomme> intermediairesNew = new List<IntermediaireConsomme>();
            RetrieveIntermediaire(intermediairesOld, intermediairesNew);

            Log.Information($"Nombre d'intermediaires consommes dans le OLD : {intermediairesOld.Count()}");
            Log.Information($"Nombre d'intermediaires consommes dans le NEW : {intermediairesNew.Count()}");
            Log.Debug(String.Empty);

            List<SupportInfoGenConsomme> infosOld = new List<SupportInfoGenConsomme>();
            List<SupportInfoGenConsomme> infosNew = new List<SupportInfoGenConsomme>();
            RetrieveInfoGen(infosOld, infosNew);

            Log.Information($"Nombre de support info gen consomme dans le OLD : {infosOld.Count()}");
            Log.Information($"Nombre de support info gen consomme dans le NEW : {infosNew.Count()}");
            Log.Debug(String.Empty);

            List<Produit> produitsOld = new List<Produit>();
            List<Produit> produitsNew = new List<Produit>();
            RetrieveProduits(produitsOld, produitsNew);

            Log.Information($"Nombre de produits dans le OLD : {produitsOld.Count()}");
            Log.Information($"Nombre de produits dans le NEW : {produitsNew.Count()}");
            Log.Debug(String.Empty);
            #endregion

            #region Analyse des données XML
            var personnesManquantes = PersonnesManquantes(personnesOld, personnesNew);

            var contratsManquants = ContratsManquants(contratsOld, contratsNew);

            var fournisseursManquants = FournisseursManquants(fournisseursOld, fournisseursNew);

            var intermediairesManquants = IntermediairesManquants(intermediairesOld, intermediairesNew);

            var infoGenManquantes = InfoGenManquantes(infosOld, infosNew);

            var produitsManquants = ProduitsManquants(produitsOld, produitsNew);

            (List<Role> rolesManquants, List<SupportComposant> supportComposantsManquants) = Analyse(contratsOld, contratsNew);

            if (personnesManquantes.Count() != 0)
            {
                Log.Information("LISTE DES PERSONNES MANQUANTES :");
                foreach (var personne in personnesManquantes)
                {
                    Log.Warning(personne.ToString());
                    Log.Information($"La personne ID = {personne.ID} possède {NbReferencesPersonneEnRole(personne.ID).ToString()} référence(s) en rôle");
                }
            }
            else Log.Information("PAS DE PERSONNE MANQUANTE.");
            Log.Debug(String.Empty);

            if (contratsManquants.Count() != 0)
            {
                Log.Information("LISTE DES CONTRATS MANQUANTS :");
                foreach (var contrat in contratsManquants)
                {
                    Log.Warning(contrat.ToString());
                }
            }
            else Log.Information("PAS DE CONTRAT MANQUANT.");
            Log.Debug(String.Empty);

            if (rolesManquants.Count != 0)
            {
                Log.Information("LISTE DES ROLES MANQUANTS :");
                foreach (var role in rolesManquants)
                {
                    Log.Warning(role.ToString());
                }
            }
            else Log.Information("PAS DE ROLES MANQUANTS");
            Log.Debug(String.Empty);

            if (supportComposantsManquants.Count() != 0)
            {
                Log.Information("LISTE DES SUPPORT_COMPOSANTS MANQUANTS :");
                foreach (var supportComposants in supportComposantsManquants)
                {
                    Log.Warning(supportComposants.ToString());
                }
            }
            else Log.Information("PAS DE SUPPORT_COMPOSANT MANQUANT");
            Log.Debug(String.Empty);

            if (fournisseursManquants.Count() != 0)
            {
                Log.Information("LISTE DES FOURNISSEUR_CONSOMMES MANQUANTS :");
                foreach (var fournisseur in fournisseursManquants)
                {
                    Log.Warning(fournisseur.ToString());
                }
            }
            else Log.Information("PAS DE FOURNISSEUR_CONSOMME MANQUANT.");
            Log.Debug(String.Empty);

            if (intermediairesManquants.Count() != 0)
            {
                Log.Information("LISTE DES INTERMEDIAIRE_CONSOMMES MANQUANTS :");
                foreach (var intermediaire in intermediairesManquants)
                {
                    Log.Warning(intermediaire.ToString());
                    Log.Information($"L'intermediaire ID = {intermediaire.Id} possède {NbReferencesIntermediaireEnContrat(intermediaire.Id).ToString()} référence() en contrat");
                }
            }
            else Log.Information("PAS D'INTERMEDIAIRE_CONSOMME MANQUANT.");
            Log.Debug(String.Empty);

            if (infoGenManquantes.Count() != 0)
            {
                Log.Information("LISTE DES SUPPORT_INFO_GEN_CONSOMMES MANQUANTS :");
                foreach (var infoGen in infoGenManquantes)
                {
                    Log.Warning(infoGen.ToString());
                }
            }
            else Log.Information("PAS DE SUPPORT_INFO_GEN_CONSOMME MANQUANT.");
            Log.Debug(String.Empty);

            if (produitsManquants.Count() != 0)
            {
                Log.Information("LISTE DES PRODUITS MANQUANTS :");
                foreach (var produit in produitsManquants)
                {
                    Log.Warning(produit.ToString());
                    Log.Information($"Le produit ID = {produit.Id} possède {NbReferencesProduitEnContrat(produit.Id).ToString()} référence(s) en contrat");
                }
            }
            else Log.Information("PAS DE PRODUIT MANQUANT.");
            Log.Debug(String.Empty);

            List<SupportInfoGenConsomme> invalidesInfoGens = AnalyseInfoGenNEW(infosNew);
            if(invalidesInfoGens.Count() != 0)
            {
                foreach(var info in invalidesInfoGens)
                {
                    Log.Warning($"[Fichier NEW] Pas d'origine_du_code_courant pour l'info_gen :  {info.ToString()}");
                }
            }

            Log.Debug(String.Empty);
            Log.Information($"FIN LOG {guid}");
            #endregion
        }

        #region Méthodes privées
        /// <summary>
        /// Retourne la lsite des info_gens du NEW n'ayant pas d'origine code courant
        /// </summary>
        /// <param name="infos">Liste des info_gens</param>
        /// <returns>Liste des info_gens sans origine de code courant</returns>
        private static List<SupportInfoGenConsomme> AnalyseInfoGenNEW(List<SupportInfoGenConsomme> infos)
        {
            List<SupportInfoGenConsomme> infosSansOrigineCodeCourant = new List<SupportInfoGenConsomme>();
            foreach(var info in infos)
            {
                if(String.IsNullOrEmpty(info.OrigineCodeCourant))
                {
                    infosSansOrigineCodeCourant.Add(info);
                }
            }
            return infosSansOrigineCodeCourant;
        }

        /// <summary>
        /// Conmpte le nombre de références d'un produit en contrat
        /// </summary>
        /// <param name="produitId">Identifiant du produit</param>
        /// <returns>Nombre de références</returns>
        private static int NbReferencesProduitEnContrat(string produitId)
        {
            var references =
                from el in oldDoc.Descendants("contrat")
                where (string)el.Attribute("ref_ID_produit") == produitId
                select el;
            var nbReferences = references.Count();
            return nbReferences;
        }

        /// <summary>
        /// Compte le nombre de références d'un intermediaire en contrat
        /// </summary>
        /// <param name="intermediaireId">Identifiant de l'intermediaire</param>
        /// <returns>Nombre de références</returns>
        private static int NbReferencesIntermediaireEnContrat(string intermediaireId)
        {
            var references =
                from el in oldDoc.Descendants("contrat")
                where (string)el.Attribute("ref_ID_conseiller") == intermediaireId
                select el;
            var nbReferences = references.Count();
            return nbReferences;
        }

        /// <summary>
        /// Retourne une liste de produits manquants
        /// </summary>
        /// <param name="oldProduits">Liste de référence</param>
        /// <param name="newProduits">Liste nouvelle</param>
        /// <returns>Liste de produits manquants</returns>
        private static List<Produit> ProduitsManquants(List<Produit> oldProduits, List<Produit> newProduits)
        {
            List<Produit> produitsManquants = new List<Produit>();
            foreach(var produit in oldProduits)
            {
                if(!newProduits.Contains(produit))
                {
                    produitsManquants.Add(produit);
                }
            }
            return produitsManquants;
        }

        /// <summary>
        /// Récupération des produits
        /// </summary>
        /// <param name="oldProduits">Liste vierge qui contiendra les produits du fichier OLD</param>
        /// <param name="newProduits">Liste vierge qui contiendra les produits du fichier NEW</param>
        private static void RetrieveProduits(List<Produit> oldProduits, List<Produit> newProduits)
        {
            FillProduits(oldProduits, oldDoc.Descendants("produit"));

            FillProduits(newProduits, newDoc.Descendants("produit"));
        }

        /// <summary>
        /// Récupération des produits
        /// </summary>
        /// <param name="Produits">Liste vierge de produits à remplir</param>
        /// <param name="xProduits">Liste des produits XML</param>
        private static void FillProduits(List<Produit> Produits, IEnumerable<XElement> xProduits)
        {
            foreach (var xProduit in xProduits)
            {
                Produit produit = new Produit
                {
                    Id = xProduit.Attribute("ID_produit")?.Value,
                    CodeRegimeFiscal = xProduit.Attribute("code_regime_fiscal")?.Value,
                    CodeTypeProduit = xProduit.Attribute("code_type_produit")?.Value,
                    Designation = xProduit.Attribute("designation")?.Value,
                    Flag = xProduit.Attribute("flag_multisupport")?.Value,
                    RefFournisseur = xProduit.Attribute("ref_ID_fournisseur")?.Value
                };
                Produits.Add(produit);
            }
        }

        /// <summary>
        /// Compte le nombre de référence d'une personne donnée en rôle de contrat
        /// </summary>
        /// <param name="personneId">Identifiant de la personne</param>
        /// <returns>Nombre de références</returns>
        private static int NbReferencesPersonneEnRole(string personneId)
        {
            var references =
                from el in oldDoc.Descendants("role")
                where (string)el.Attribute("ref_ID_personne") == personneId
                select el;
            var nbReferences = references.Count();
            return nbReferences;
        }

        /// <summary>
        /// Retiurne une liste d'informations générales manquantes
        /// </summary>
        /// <param name="oldInfos">Liste de référence</param>
        /// <param name="newInfos">Liste nouvelle</param>
        /// <returns>Liste d'informations générales manquantes</returns>
        private static List<SupportInfoGenConsomme> InfoGenManquantes(List<SupportInfoGenConsomme> oldInfos, List<SupportInfoGenConsomme> newInfos)
        {
            List<SupportInfoGenConsomme> infoGenManquantes = new List<SupportInfoGenConsomme>();
            foreach(var infoGen in oldInfos)
            {
                if(!newInfos.Contains(infoGen))
                {
                    infoGenManquantes.Add(infoGen);
                }
            }
            return infoGenManquantes;
        }

        /// <summary>
        /// Récupération des informations générales
        /// </summary>
        /// <param name="oldInfos">Liste vierge qui contiendra les info_gen du fichier OLD</param>
        /// <param name="newInfos">Liste vierge qui contiendra les info_gen du fichier NEW</param>
        private static void RetrieveInfoGen(List<SupportInfoGenConsomme> oldInfos, List<SupportInfoGenConsomme> newInfos)
        {
            FillInfoGens(oldInfos, oldDoc.Descendants("support_info_gen_consomme"));

            FillInfoGens(newInfos, newDoc.Descendants("support_info_gen_consomme"));
        }

        /// <summary>
        /// Récupération des informations générales
        /// </summary>
        /// <param name="InfoGens">Liste vierge d'informations à remplir</param>
        /// <param name="xInfoGens">Liste d'informations XML</param>
        private static void FillInfoGens(List<SupportInfoGenConsomme> InfoGens, IEnumerable<XElement> xInfoGens)
        {
            foreach (var xInfo in xInfoGens)
            {
                SupportInfoGenConsomme info = new SupportInfoGenConsomme
                {
                    Id = xInfo.Attribute("ID_support_info_generale")?.Value,
                    CodeCourant = xInfo.Attribute("code_courant")?.Value,
                    CodePrecedent = xInfo.Attribute("code_precedent")?.Value,
                    Designation = xInfo.Attribute("designation")?.Value,
                    IdFournisseur = xInfo.Attribute("ref_ID_fournisseur")?.Value,
                    OrigineCodeCourant = xInfo.Attribute("origine_du_code_courant")?.Value,
                    OrigineCodePrecedent = xInfo.Attribute("origine_du_code_precedent")?.Value
                };
                InfoGens.Add(info);
            }
        }

        /// <summary>
        /// Retourne une liste des intermediaires manquants
        /// </summary>
        /// <param name="oldIntermediaires">Liste de référence</param>
        /// <param name="newIntermediaires">Liste nouvelles</param>
        /// <returns>Liste des intermediaires manquants</returns>
        private static List<IntermediaireConsomme> IntermediairesManquants(List<IntermediaireConsomme> oldIntermediaires, List<IntermediaireConsomme> newIntermediaires)
        {
            List<IntermediaireConsomme> intermediairesManquants = new List<IntermediaireConsomme>();
            foreach(var intermediaire in oldIntermediaires)
            {
                if(!newIntermediaires.Contains(intermediaire))
                {
                    intermediairesManquants.Add(intermediaire);
                }
            }
            return intermediairesManquants;
        }

        /// <summary>
        /// Récupération des intermediaires
        /// </summary>
        /// <param name="intermediairesOld">Liste vierge qui contiendra les intermediaires du fichier OLD</param>
        /// <param name="intermediairesNew">Liste vierge qui contiendra les intermediaires du fichier NEW</param>
        private static void RetrieveIntermediaire(List<IntermediaireConsomme> intermediairesOld, List<IntermediaireConsomme> intermediairesNew)
        {
            FillIntermediaires(intermediairesOld, oldDoc.Descendants("intermediaire_consomme"));

            FillIntermediaires(intermediairesNew, newDoc.Descendants("intermediaire_consomme"));
        }

        /// <summary>
        /// Récupération des intermediaires
        /// </summary>
        /// <param name="Intermediaires">Liste vierge d'intermediaires à remplir</param>
        /// <param name="xIntermediaires">Liste d'intermediaires XML</param>
        private static void FillIntermediaires(List<IntermediaireConsomme> Intermediaires, IEnumerable<XElement> xIntermediaires)
        {
            foreach (var xIntermediaire in xIntermediaires)
            {
                IntermediaireConsomme intermediaire = new IntermediaireConsomme
                {
                    Id = xIntermediaire.Attribute("ID_intermediaire")?.Value,
                    Code = xIntermediaire.Attribute("code_interne1")?.Value,
                    Denomination = xIntermediaire.Attribute("denomination")?.Value
                };
                Intermediaires.Add(intermediaire);
            }
        }

        /// <summary>
        /// Retourne une liste de fournisseurs manquants
        /// </summary>
        /// <param name="oldFournisseurs">Liste de référence</param>
        /// <param name="newFournisseurs">Liste nouvelle</param>
        /// <returns>Liste de fournisseurs manquants</returns>
        private static List<FournisseurConsomme> FournisseursManquants(List<FournisseurConsomme> oldFournisseurs, List<FournisseurConsomme> newFournisseurs)
        {
            List<FournisseurConsomme> fournisseursManquants = new List<FournisseurConsomme>();
            foreach(var fournisseur in oldFournisseurs)
            {
                if(!newFournisseurs.Contains(fournisseur))
                {
                    fournisseursManquants.Add(fournisseur);
                }
            }
            return fournisseursManquants;
        }

        /// <summary>
        /// Récupération des fournisseurs
        /// </summary>
        /// <param name="fournisseursOld">Liste vierge qui contiendra les frournisseurs du fichier OLD</param>
        /// <param name="fournisseursNew">Liste vierge qui contiendra les frournisseurs du fichier NEW</param>
        private static void RetrieveFournisseurs(List<FournisseurConsomme> fournisseursOld, List<FournisseurConsomme> fournisseursNew)
        {
            FillFournisseurs(fournisseursOld, oldDoc.Descendants("fournisseur_consomme"));

            FillFournisseurs(fournisseursNew, newDoc.Descendants("fournisseur_consomme"));
        }

        /// <summary>
        /// Récupération des fournisseurs
        /// </summary>
        /// <param name="Fournisseurs">Liste vierge de fournisseurs à remplir</param>
        /// <param name="xFournisseurs">Lsite de fournisseurs XML</param>
        private static void FillFournisseurs(List<FournisseurConsomme> Fournisseurs, IEnumerable<XElement> xFournisseurs)
        {
            foreach (var xFournisseur in xFournisseurs)
            {
                FournisseurConsomme fournisseur = new FournisseurConsomme
                {
                    Id = xFournisseur.Attribute("ID_fournisseur")?.Value,
                    Libelle = xFournisseur.Attribute("Libelle_court")?.Value
                };
                Fournisseurs.Add(fournisseur);
            }
        }

        /// <summary>
        /// Retourne une liste de support_composants manquants pour un contrat donné
        /// </summary>
        /// <param name="oldSupportComposants">Liste de référence</param>
        /// <param name="newSupportComposants">Liste nouvelle</param>
        /// <param name="idContrat">Identifiant du contrat</param>
        /// <returns>Liste des support_composants manquants</returns>
        private static List<SupportComposant> AnalyseSupportComposants(List<SupportComposant> oldSupportComposants, List<SupportComposant> newSupportComposants, string idContrat)
        {
            foreach(var supportComposant in oldSupportComposants)
            {
                if(!newSupportComposants.Contains(supportComposant))
                {
                    supportComposant.IdContrat = idContrat;
                    supportComposantsManquants.Add(supportComposant);
                }
            }
            return supportComposantsManquants;
        }

        /// <summary>
        /// Retourne une lsite de rôles manquants pour un contrat donné
        /// </summary>
        /// <param name="oldRoles">Liste de référence</param>
        /// <param name="newRoles">Liste nouvelle</param>
        /// <param name="IdContrat">Identifiant du contrat</param>
        /// <returns>Liste des rôles manquants</returns>
        private static List<Role> AnalyseRoles(List<Role> oldRoles, List<Role> newRoles, string IdContrat)
        {
            foreach(var role in oldRoles)
            {
                if(!newRoles.Contains(role))
                {
                    role.IdContrat = IdContrat;
                    rolesManquants.Add(role);
                }
            }
            return rolesManquants;
        }

        /// <summary>
        /// Analyse les rôles et support_composants de contrats
        /// </summary>
        /// <param name="oldContrats">Liste de référence</param>
        /// <param name="newContrats">Liste nouvelle</param>
        /// <returns>Tuple des rôles manquants et des suoport_composants manquants</returns>
        private static (List<Role> roles, List<SupportComposant> supportComposants) Analyse(List<Contrat> oldContrats, List<Contrat> newContrats)
        {
            List<Role> rolesManquants = new List<Role>();
            List<SupportComposant> supportComposantsManquants = new List<SupportComposant>();
            foreach(var contrat in oldContrats)
            {
                if(newContrats.Contains(contrat))
                {
                    var contratNew = newContrats.Find(c => c.ID == contrat.ID);
                    //Compter le nombre de Roles
                    var oldRoles = contrat.Roles;
                    var newRoles = contratNew.Roles;
                    if(oldRoles.Count() != newRoles.Count())
                    {
                        Log.Warning($"Le nombre de rôles pour le contrat {contrat.ID} diffère entre le OLD ({oldRoles.Count().ToString()}) et le NEW ({newRoles.Count().ToString()})");
                        Log.Debug(String.Empty);
                    }
                    //Compter le nombre de SupportComposants
                    var oldSupportComposants = contrat.SupportComposants;
                    var newSupportComposants = contratNew.SupportComposants;
                    if(oldSupportComposants.Count() != newSupportComposants.Count())
                    {
                        Log.Warning($"Le nombre de supports composants pour le contrat {contrat.ID} diffère entre le OLD ({oldSupportComposants.Count().ToString()}) et le NEW ({newSupportComposants.Count().ToString()})");
                        Log.Debug(String.Empty);
                    }
                    // Comparer les ID des rôles
                    rolesManquants = AnalyseRoles(oldRoles, newRoles, contrat.ID);
                    supportComposantsManquants = AnalyseSupportComposants(oldSupportComposants, newSupportComposants, contrat.ID);
                }
            }
            return (rolesManquants, supportComposantsManquants);
        }

        /// <summary>
        /// Retourne une liste des contrats manquants
        /// </summary>
        /// <param name="oldContrats">Liste de référence</param>
        /// <param name="newContrats">Liste nouvelle</param>
        /// <returns>Liste des contrats manquants</returns>
        private static List<Contrat> ContratsManquants(List<Contrat> oldContrats, List<Contrat> newContrats)
        {
            List<Contrat> contratsManquants = new List<Contrat>();
            foreach (var contrat in oldContrats)
            {
                if(!newContrats.Contains(contrat))
                {
                    contratsManquants.Add(contrat);
                }
            }
            return contratsManquants;
        }

        /// <summary>
        /// Retourne une liste des personnes manquantes
        /// </summary>
        /// <param name="oldPersonnes">Liste des référence</param>
        /// <param name="newPersonnes">Liste nouvelle</param>
        /// <returns>Liste des personnes manquantes</returns>
        private static List<Personne> PersonnesManquantes(List<Personne> oldPersonnes, List<Personne> newPersonnes)
        {
            List<Personne> personnesManquantes = new List<Personne>();
            foreach(var personne in oldPersonnes)
            {
                if(!newPersonnes.Contains(personne))
                {
                    personnesManquantes.Add(personne);
                }
            }
            return personnesManquantes;
        }

        /// <summary>
        /// Récupération des support_composants d'un contrat
        /// </summary>
        /// <param name="xContrat">Elemenent XML représentant un support_composant</param>
        /// <returns>Liste des support_composants d'un contrat</returns>
        private static List<SupportComposant> RetrieveSupporComposants(XElement xContrat)
        {
            List<SupportComposant> supportComposants = new List<SupportComposant>();
            var xSupportComposants = xContrat.Descendants("support_composant");
            foreach(var xSupportComposant in xSupportComposants)
            {
                SupportComposant supportComposant = new SupportComposant
                {
                    DateCotation = xSupportComposant.Attribute("date_cotation")?.Value,
                    DateValorisation = xSupportComposant.Attribute("date_valorisation")?.Value,
                    ID = xSupportComposant.Attribute("ID_support_composant")?.Value,
                    IDInfoGenerale = xSupportComposant.Attribute("ref_ID_support_info_generale")?.Value,
                    Montant = xSupportComposant.Attribute("montant_en_euro")?.Value,
                    NbParts = xSupportComposant.Attribute("nombre_de_parts")?.Value,
                    ValeurCotation = xSupportComposant.Attribute("valeur_cotation_euro")?.Value
                };
                supportComposants.Add(supportComposant);
            }
            return supportComposants;
        }

        /// <summary>
        /// Récupération des rôles d'un contrat
        /// </summary>
        /// <param name="xContrat">Element XML représentant un contrat</param>
        /// <returns>Liste des rôles d'un contrat</returns>
        private static List<Role> RetrieveRoles(XElement xContrat)
        {
            List<Role> roles = new List<Role>();
            var xRoles = xContrat.Descendants("role");
            foreach(var xRole in xRoles)
            {
                Role role = new Role
                {
                    Code = xRole.Attribute("code_role")?.Value,
                    ID = xRole.Attribute("ID_role")?.Value,
                    IdPersonne = xRole.Attribute("ref_ID_personne")?.Value
                };
                roles.Add(role);
            }
            return roles;
        }

        /// <summary>
        /// Récupération des contrats
        /// </summary>
        /// <param name="contratsOld">Liste vierge qui contiendra les contrats du fichier OLD</param>
        /// <param name="contratsNew">Liste vierge qui contiendra les contrats du fichier NEW</param>
        private static void RetrieveContracts(List<Contrat> contratsOld, List<Contrat> contratsNew)
        {
            FillContracts(contratsOld, oldDoc.Descendants("contrat"));

            FillContracts(contratsNew, newDoc.Descendants("contrat"));
        }

        /// <summary>
        /// Récupération des contrats
        /// </summary>
        /// <param name="Contrats">Liste vierge de contrats à remplir</param>
        /// <param name="xContrats">Liste de contrats XML</param>
        private static void FillContracts(List<Contrat> Contrats, IEnumerable<XElement> xContrats)
        {
            foreach (var xContrat in xContrats)
            {
                Contrat contrat = new Contrat
                {
                    CodeEtat = xContrat.Attribute("code_etat_contrat")?.Value,
                    DateEffet = xContrat.Attribute("date_effet_contrat")?.Value,
                    DateSituation = xContrat.Attribute("date_situation")?.Value,
                    ID = xContrat.Attribute("ID_contrat")?.Value,
                    Numero = xContrat.Attribute("num_contrat")?.Value,
                    refConseiller = xContrat.Attribute("ref_ID_conseiller")?.Value,
                    RefProduit = xContrat.Attribute("ref_ID_produit")?.Value,
                    RegimeFiscal = xContrat.Attribute("code_regime_fiscal")?.Value
                };
                contrat.Roles = RetrieveRoles(xContrat);
                contrat.SupportComposants = RetrieveSupporComposants(xContrat);
                Contrats.Add(contrat);
            }
        }

        /// <summary>
        /// Récupération des personnes
        /// </summary>
        /// <param name="personnesOld">Liste vierge qui contiendra les personnes du fichier OLD</param>
        /// <param name="personnesNew">Liste vierge qui contiendra les personnes du fichier NEW</param>
        private static void RetrievePersonnes(List<Personne> personnesOld, List<Personne> personnesNew)
        {
            FillPersonnes(personnesOld, oldDoc.Descendants("personne"));

            FillPersonnes(personnesNew, newDoc.Descendants("personne"));
        }

        /// <summary>
        /// Récupération des personnes
        /// </summary>
        /// <param name="Personnes">Liste vierge de personnes à remplir</param>
        /// <param name="xPersonnes">Liste de personnes XML</param>
        private static void FillPersonnes(List<Personne> Personnes, IEnumerable<XElement> xPersonnes)
        {
            foreach (var xPersonne in xPersonnes)
            {
                Personne personne = new Personne
                {
                    Nom = xPersonne.Attribute("nom_patronymique")?.Value,
                    Prenoms = xPersonne.Attribute("prenoms")?.Value,
                    Civilite = xPersonne.Attribute("code_civilite")?.Value,
                    ID = xPersonne.Attribute("ID_personne")?.Value
                };
                Personnes.Add(personne);
            }
        }
        #endregion
    }
}
