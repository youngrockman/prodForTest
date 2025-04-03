using NUnit.Framework;
using System;
using System.IO;

namespace prodForTest.Tests
{
    [TestFixture]
    public class AccountTestsWithoutFiles
    {
        private account testAccount;
        private StringWriter consoleOutput;

        [SetUp]
        public void Setup()
        {
            testAccount = new account();
            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
        }
        
        
        

        [Test]
        public void Otk_ValidInput_ShouldSetInitialBalance()
        {
            // Подготовка
            Console.SetIn(new StringReader("Иванов Иван\n1500\n"));

            // Действие
            testAccount.otk();

            // Проверка
            StringAssert.Contains("1500", consoleOutput.ToString());
        }

        [Test]
        public void Otk_InvalidInput_ShouldSetInitialBalance()
        {
            Console.SetIn(new StringReader("Иванов Ива\n999\n"));
            
            testAccount.otk();

            StringAssert.Contains("Сумма слишком мала, попробуйте ещё раз!", consoleOutput.ToString());

        }


        [Test]
        public void TopUp_AddFloatValue_ShouldSetBalance()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("678,78\n"));
            
            testAccount.top_up();
            
            Assert.AreEqual(1678,78,testAccount.sum);
            
        }
        

        [Test]
        public void TopUp_Add500To1000_ShouldResult1500()
        {
            
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("500\n"));

           
            testAccount.top_up();

            
            Assert.AreEqual(1500, testAccount.sum);
        }

        [Test]
        public void Umen_Subtract300From1000_ShouldResult700()
        {
            
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("300\n"));

            
            testAccount.umen();

            
            Assert.AreEqual(700, testAccount.sum);
        }


        [Test]
        public void Umen_Subtract10001From1000_ShouldThrowExeption()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("1001\n"));
            
            testAccount.umen();
            
            Assert.Catch<Exception>(() => testAccount.umen());
        }

        [Test]
        public void Obnul_AnyBalance_ShouldResultZero()
        {
          
            testAccount.sum = 1000;

            testAccount.obnul();

           
            Assert.AreEqual(0, testAccount.sum);
        }

        [Test]
        public void Perevod_ValidTransfer_ShouldUpdateBalance()
        {
            
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("300\n12345\n"));

            
            testAccount.perevod();

            
            Assert.AreEqual(700, testAccount.sum);
        }

        [TearDown]
        public void Cleanup()
        {
            consoleOutput.Dispose();
        }
    }
}