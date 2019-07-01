using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;

namespace ClaimsExemple
{
    class Program
    {
        static void Main(string[] args)
        {
            Setup();
        }

        private static void Setup()
                {
                    //  Els claims defineixen què o qui és l'identity, no el que pot fer
                    IList<Claim> claimCollection = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, "Pep Pi")
                        , new Claim(ClaimTypes.Country, "Sweden")
                        , new Claim(ClaimTypes.Gender, "M")
                        , new Claim(ClaimTypes.Surname, "Pi")
                        , new Claim(ClaimTypes.Email, "pipi@aliga.edu")
                        , new Claim(ClaimTypes.Role, "IT")
                    };



                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claimCollection);

                    Console.WriteLine(" Està ja autenticat? {0}", claimsIdentity.IsAuthenticated);

                    claimsIdentity = new ClaimsIdentity(claimCollection, "Aliga3");
                    Console.WriteLine(" El poso a Aliga3, està ja autenticat? {0}", claimsIdentity.IsAuthenticated);


                    // Si es crea un ClaimsIdentity i es defineix un AutheticationType en els constructor
                    // el valor de IsAuthenticated és sempre `true`.
                    //
                    // - L'usuari sempre ha de teir un `AuthenticationType` per poder ser considerat identificat.
                    // - Per tant no es pot tenir un usuari identificat sense AuthenticationType
                    Console.WriteLine("---> AuthenticationType {0}", claimsIdentity.AuthenticationType);


                    // No sempre està clar què coi és `Name` (teòricament és el que serveix per identificar
                    // a l'usuari) però pot ser un username, correu electrònic, display name, ...
                    // per definir-ho exactament es poden passar dos paràmetres al constructor que que serveixen per
                    // definir què és el que es fa servir de `Name` i què és el `Role`
                    claimsIdentity = new ClaimsIdentity(claimCollection, "Aliga3", ClaimTypes.Name, ClaimTypes.Role);
                    ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

                    // El poso al thread per poder-lo recuperar. En ASP.NET no cal perquè s'injecta amb el context
                    Thread.CurrentPrincipal = principal;
                    Show();


                    claimsIdentity = new ClaimsIdentity(claimCollection, "Aliga3", ClaimTypes.Email, ClaimTypes.Role);
                    principal = new ClaimsPrincipal(claimsIdentity);
                    Thread.CurrentPrincipal = principal;
                    Show();

                    // En versions més modernes es fa servir el ClaimsPrincipal. Que com a característica té que
                    // pot tenir diverses ClaimIdentity (exemple clàssic: passaport, bitllet d'avio)
                    //
                    //  - IIdentity Identity { get; }  <- Apunta la primera identity
                    //  - public IEnumerable<ClaimsIdentity> Identities { get; }
                    //  - public IEnumerable<Claim> Claims { get; }  <- El principal té Claims de totes les identities

                    ShowNew();


                }

        private static void Show()
        {
            // Recuperar-lo del thread (en ASP.NET no caldria). Es pot col·locar en un IPrincipal
            IPrincipal currentPrincipal = Thread.CurrentPrincipal;

            // Imprimir valors
            Console.WriteLine("Identificador {0}", currentPrincipal.Identity.Name);
        }

        private static void ShowNew()
        {
            ClaimsPrincipal currentClaimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            // és idèntic que ...
            //
            // En ASP.NET core el HttpContext té una variable User que és el ClaimsPrincipal (IPrincipal)

            // Teòricament en comptes de recuperar-lo del Thread es pot recuperar:
            //
            // ClaimsPrincipal currentClaimsPrincipal = ClaimsPrincipal.Current;


            Claim nameClaim = currentClaimsPrincipal.FindFirst(ClaimTypes.Name);
            Console.WriteLine("Name: {0}", nameClaim.Value);

            foreach (ClaimsIdentity ci in currentClaimsPrincipal.Identities)
            {
                Console.WriteLine($"   Identities: ${ci.AuthenticationType}, ${ci.Name}");
            }
        }
    }
}
