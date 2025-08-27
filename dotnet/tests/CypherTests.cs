using data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using model;
using System.Diagnostics;

namespace tests
{
    [TestClass]
    public class CtpherTests
    {
        const string linha = "=============================================================";

        [TestMethod]
        public void Run()
        {            
            var plainText = "Ramon da Silva Pacheco ramonpacheco@outlook.com09931768606";           

            var cypheredText = SearchableHelper.Build(plainText, Usuario.SearchableScope);

            Debug.WriteLine(linha);
            Debug.WriteLine($"plainText: {plainText}");
            Debug.WriteLine($"cypheredText: {cypheredText}");
            Debug.WriteLine(linha);
            
            var filterPlainText = "Pacheco";            
            var filterCypheredText = SearchableHelper.Build(filterPlainText, Usuario.SearchableScope);

            Debug.WriteLine(linha);
            Debug.WriteLine($"filterPlainText: {filterPlainText}");
            Debug.WriteLine($"filterCypheredText: {filterCypheredText}");
            Debug.WriteLine(linha);

            Assert.IsTrue(cypheredText.Contains(filterCypheredText));
        } 
    }     
}
