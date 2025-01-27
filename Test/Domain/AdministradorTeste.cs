using System.Security.Cryptography.X509Certificates; 


namespace Test.Domain;

[TestClass]
public class AdministradorTeste
{
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            // Arrange
            var administrador = new Administrador();
            administrador.Nome = "Administrador";
            administrador.Email = "";

        Assert.AreEqual(administrador.Nome, "Administrador");
        }
}