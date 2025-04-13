using NUnit.Framework;
using System;
using System.IO;

namespace prodForTest.Tests
{
    [TestFixture]
    public class AccountTests
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
        public void Otk_WithValidInput_ShouldCreateAccount()
        {
            // Подготовка
            Console.SetIn(new StringReader("Петров Петр Петрович\n1500\n"));

            // Действие
            testAccount.otk();

            // Проверка
            Assert.AreEqual("Петров Петр Петрович", testAccount.name);
            Assert.AreEqual(1500, testAccount.sum);
            Assert.IsNotNull(testAccount.num);
        }

        [Test]
        public void Otk_WithLowInitialBalance_ShouldShowErrorMessage()
        {
            Console.SetIn(new StringReader("Сидоров Сидор\n500\nСидоров Сидор\n2000\n"));
            
            testAccount.otk();

            StringAssert.Contains("Сумма слишком мала", consoleOutput.ToString());
            Assert.AreEqual(2000, testAccount.sum);
        }

        [Test]
        public void NumGen_ShouldGenerate20DigitNumber()
        {
            testAccount.num_gen();
            
            Assert.AreEqual(20, testAccount.num.Length);
            Assert.IsTrue(long.TryParse(testAccount.num, out _));
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
        public void TopUp_AddFloatValue_ShouldSetBalance()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("678,78\n"));
            
            testAccount.top_up();
            
            Assert.AreEqual(1678.78f, testAccount.sum);
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
        public void Umen_SubtractMoreThanBalance_ShouldNotChangeBalance()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("1500\n"));
            
            testAccount.umen();
            
            Assert.AreEqual(1000, testAccount.sum);
            StringAssert.Contains("Недостаточно средств", consoleOutput.ToString());
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
            Console.SetIn(new StringReader("300\n0\n"));
            
            testAccount.perevod();
            
            Assert.AreEqual(700, testAccount.sum);
            Assert.AreEqual(300, testAccount.summ);
        }

        [Test]
        public void Perevod_InsufficientFunds_ShouldShowErrorMessage()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("1500\n0\n"));
            
            testAccount.perevod();
            
            Assert.AreEqual(1000, testAccount.sum);
            StringAssert.Contains("Недостаточно средств", consoleOutput.ToString());
        }

        [Test]
        public void Show_ShouldDisplayCorrectAccountInfo()
        {
            testAccount.num = "12345678901234567890";
            testAccount.name = "Иванов Иван";
            testAccount.sum = 5000;
            testAccount.putt = "test_account.txt";
            
            testAccount.show();
            
            StringAssert.Contains("Номер счёта: 12345678901234567890", consoleOutput.ToString());
            StringAssert.Contains("ФИО владельца: Иванов Иван", consoleOutput.ToString());
            StringAssert.Contains("Баланс: 5000 р.", consoleOutput.ToString());
            
            // Очистка
            File.Delete("test_account.txt");
        }
        
        [Test]
        public void Otk_MultipleAttempts_ShouldSucceedWithValidInput()
        {
            Console.SetIn(new StringReader("Неправильное имя\n\nСмирнов Алексей\n500\nСмирнов Алексей\n2000\n"));
        
            testAccount.otk();
        
            Assert.AreEqual("Смирнов Алексей", testAccount.name);
            Assert.AreEqual(2000, testAccount.sum);
        }

        [Test]
        public void NumGen_TwoAccounts_ShouldGenerateDifferentNumbers()
        {
            var account1 = new account();
            var account2 = new account();
        
            account1.num_gen();
            account2.num_gen();
        
            Assert.AreNotEqual(account1.num, account2.num);
        }

        [Test]
        public void TopUp_MultipleOperations_ShouldCalculateCorrectBalance()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("500\n300\n200\n"));
        
            testAccount.top_up();
            testAccount.top_up();
            testAccount.top_up();
        
            Assert.AreEqual(2000, testAccount.sum);
        }

        [Test]
        public void Umen_AttemptToWithdrawNegativeAmount_ShouldNotChangeBalance()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("-100\n"));
        
            testAccount.umen();
        
            Assert.AreEqual(1000, testAccount.sum);
            StringAssert.Contains("Неверная сумма", consoleOutput.ToString());
        }
        
        
        [Test]
        public void Perevod_ToSameAccount_ShouldShowErrorMessage()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("500\n0\n"));
        
            testAccount.perevod();
        
            StringAssert.Contains("Нельзя перевести на тот же счет", consoleOutput.ToString());
        }
        

        [TearDown]
        public void Cleanup()
        {
            consoleOutput.Dispose();
        }
    }
}