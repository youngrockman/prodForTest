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

        public void Otk_NameWithNumbers_ShoulShowErrorMessage()
        {
            Console.SetIn(new StreamReader("1231!!32432;\n500\n"));
            
            testAccount.otk();
            
            Assert.That(consoleOutput.ToString(), Is.EqualTo("Неверное имя пользователя"));
            
        }

        [Test]

        public void Otk_BalanceIsNotDigits_ShoulShowErrorMessage()
        {
            Console.SetIn(new StreamReader("Сидор Сидоров\nabc\n"));
            
            testAccount.otk();
            
            Assert.That(consoleOutput.ToString(), Is.EqualTo("Неверный формат ввода"));
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
        
        [Test]
        public void Otk_EmptyName_ShouldPromptAgain()
        {
            Console.SetIn(new StringReader("\n \nИванов Иван\n2000\n"));
            
            testAccount.otk();
            
            StringAssert.Contains("Пожалуйста, введите своё ФИО", consoleOutput.ToString());
            Assert.AreEqual("Иванов Иван", testAccount.name);
        }

        [Test]
        public void Otk_InvalidSumFormat_ShouldHandleGracefully()
        {
            Console.SetIn(new StringReader("Иванов Иван\nне число\n2000\n"));
            
            testAccount.otk();
            
            StringAssert.Contains("Сумма слишком мала", consoleOutput.ToString());
            Assert.AreEqual(2000, testAccount.sum);
        }

        [Test]
        public void TopUp_InvalidInput_ShouldNotChangeBalance()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("не число\n500\n"));
            
            testAccount.top_up();
            
            Assert.AreEqual(1500, testAccount.sum);
        }

        [Test]
        public void Umen_ZeroAmount_ShouldNotChangeBalance()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("0\n"));
            
            testAccount.umen();
            
            Assert.AreEqual(1000, testAccount.sum);
        }

        [Test]
        public void Show_FileCreation_ShouldCreateValidFile()
        {
            testAccount.num = "11112222333344445555";
            testAccount.name = "Петров Петр";
            testAccount.sum = 3000;
            testAccount.putt = "test_show.txt";
            
            testAccount.show();
            
            Assert.True(File.Exists("test_show.txt"));
            var fileContent = File.ReadAllText("test_show.txt");
            StringAssert.Contains("11112222333344445555", fileContent);
            StringAssert.Contains("Петров Петр", fileContent);
            StringAssert.Contains("3000", fileContent);
            
            File.Delete("test_show.txt");
        }

        [Test]
        public void Perevod_NegativeAmount_ShouldShowErrorMessage()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("-100\n0\n"));
            
            testAccount.perevod();
            
            Assert.AreEqual(1000, testAccount.sum);
            StringAssert.Contains("Неверная сумма", consoleOutput.ToString());
        }

        [Test]
        public void Perevod_InvalidAccountIndex_ShouldShowErrorMessage()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("500\n9999\n"));
            
            testAccount.perevod();
            
            Assert.AreEqual(1000, testAccount.sum);
            StringAssert.Contains("Неверный индекс счета", consoleOutput.ToString());
        }

        [Test]
        public void Pereschet_ShouldUpdateFileCorrectly()
        {
            // Подготовка тестового файла
            testAccount.putt = "test_pereschet.txt";
            File.WriteAllText(testAccount.putt, "Номер счёта: 1111\nФИО владельца: Тест\nБаланс: 1000 р.");
            
            testAccount.num = "2222";
            testAccount.name = "Новое имя";
            testAccount.sum = 2000;
            
            testAccount.pereschet();
            
            var fileContent = File.ReadAllText(testAccount.putt);
            StringAssert.Contains("2222", fileContent);
            StringAssert.Contains("Новое имя", fileContent);
            StringAssert.Contains("2000", fileContent);
            
            File.Delete(testAccount.putt);
        }
        
        [Test]
        public void Otk_NameWithSpecialCharacters_ShouldAcceptInput()
        {
            Console.SetIn(new StringReader("Иванов-Петров Иван\n2000\n"));
            
            testAccount.otk();
            
            Assert.AreEqual("Иванов-Петров Иван", testAccount.name);
            Assert.AreEqual(2000, testAccount.sum);
        }

        [Test]
        public void Otk_MaximumInitialBalance_ShouldAcceptInput()
        {
            Console.SetIn(new StringReader("Тестовый Клиент\n999999999\n"));
            
            testAccount.otk();
            
            Assert.AreEqual(999999999, testAccount.sum);
        }

        [Test]
        public void TopUp_WithPrecisionLoss_ShouldHandleCorrectly()
        {
            testAccount.sum = 1000.001f;
            Console.SetIn(new StringReader("0.002\n"));
            
            testAccount.top_up();
            
            Assert.AreEqual(1000.003f, testAccount.sum, 0.0001f);
        }

        [Test]
        public void Umen_ExactBalanceWithdrawal_ShouldResultZero()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("1000\n"));
            
            testAccount.umen();
            
            Assert.AreEqual(0, testAccount.sum);
        }
        
        

        [Test]
        public void NumGen_AfterAccountModification_ShouldNotChange()
        {
            testAccount.num_gen();
            string originalNum = testAccount.num;
            
            testAccount.sum = 5000;
            testAccount.name = "Новое имя";
            
            Assert.AreEqual(originalNum, testAccount.num);
        }

        [Test]
        public void Obnul_ThenTopUp_ShouldWorkCorrectly()
        {
            testAccount.sum = 1000;
            testAccount.obnul();
            
            Console.SetIn(new StringReader("500\n"));
            testAccount.top_up();
            
            Assert.AreEqual(500, testAccount.sum);
        }
        

        [Test]
        public void Otk_WithVeryLongName_ShouldTruncateOrAccept()
        {
            string longName = new string('A', 1000);
            Console.SetIn(new StringReader($"{longName}\n2000\n"));
            
            testAccount.otk();
            
            Assert.AreEqual(longName, testAccount.name);
        }

        [Test]
        public void TopUp_ThenImmediateWithdrawal_ShouldMaintainConsistency()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("500\n"));
            testAccount.top_up();
            
            Console.SetIn(new StringReader("300\n"));
            testAccount.umen();
            
            Assert.AreEqual(1200, testAccount.sum);
        }

        [Test]
        public void Show_MultipleCalls_ShouldNotDuplicateFileContent()
        {
            testAccount.num = "123";
            testAccount.name = "Test";
            testAccount.sum = 100;
            testAccount.putt = "multishow_test.txt";
            
            testAccount.show();
            testAccount.show();
            
            var lines = File.ReadAllLines("multishow_test.txt");
            Assert.AreEqual(3, lines.Length); // 3 строки, а не 6
            File.Delete("multishow_test.txt");
        }
        

        [Test]
        public void TopUp_WithVeryLargeAmount_ShouldHandleCorrectly()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("999999999\n"));
            
            testAccount.top_up();
            
            Assert.AreEqual(1000000000, testAccount.sum);
        }

        [Test]
        public void Umen_WithDecimalAmount_ShouldCalculatePrecisely()
        {
            testAccount.sum = 1000.50f;
            Console.SetIn(new StringReader("200.25\n"));
            
            testAccount.umen();
            
            Assert.AreEqual(800.25f, testAccount.sum, 0.001f);
        }

        [Test]
        public void Perevod_WithDecimalAmount_ShouldTransferPrecisely()
        {
            testAccount.sum = 1000.75f;
            Console.SetIn(new StringReader("300.25\n0\n"));
            
            testAccount.perevod();
            
            Assert.AreEqual(700.50f, testAccount.sum, 0.001f);
            Assert.AreEqual(300.25f, testAccount.summ, 0.001f);
        }

        [Test]
        public void Show_WithZeroBalance_ShouldDisplayCorrectInfo()
        {
            testAccount.num = "00000000000000000000";
            testAccount.name = "Нулевой Клиент";
            testAccount.sum = 0;
            testAccount.putt = "zero_balance_test.txt";
            
            testAccount.show();
            
            var fileContent = File.ReadAllText("zero_balance_test.txt");
            StringAssert.Contains("Баланс: 0 р.", fileContent);
            File.Delete("zero_balance_test.txt");
        }

        [Test]
        public void FileOperations_WithInvalidPath_ShouldHandleGracefully()
        {
            testAccount.putt = "invalid/path/test.txt";
            testAccount.num = "123";
            testAccount.name = "Test";
            testAccount.sum = 100;
            
            Assert.Throws<DirectoryNotFoundException>(() => testAccount.show());
        }

        [Test]
        public void Otk_WithMixedCaseName_ShouldPreserveCase()
        {
            Console.SetIn(new StringReader("ИвАнОв ИвАн\n2000\n"));
            
            testAccount.otk();
            
            Assert.AreEqual("ИвАнОв ИвАн", testAccount.name);
        }

        [Test]
        public void NumGen_ShouldNotStartWithZero()
        {
            testAccount.num_gen();
            
            Assert.AreNotEqual('0', testAccount.num[0]);
        }

        [Test]
        public void TopUp_ThenObnul_ShouldResultZero()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("500\n"));
            testAccount.top_up();
            
            testAccount.obnul();
            
            Assert.AreEqual(0, testAccount.sum);
        }

        [Test]
        public void MultipleOperations_Sequence_ShouldMaintainCorrectBalance()
        {
            // Открытие счета
            Console.SetIn(new StringReader("Тестовый Клиент\n2000\n"));
            testAccount.otk();
            
            // Пополнение
            Console.SetIn(new StringReader("500\n"));
            testAccount.top_up();
            
            // Снятие
            Console.SetIn(new StringReader("300\n"));
            testAccount.umen();
            
            // Перевод
            Console.SetIn(new StringReader("200\n0\n"));
            testAccount.perevod();
            
            // Обнуление
            testAccount.obnul();
            
            Assert.AreEqual(0, testAccount.sum);
        }
        

        [Test]
        public void Pereschet_WithDifferentLineEndings_ShouldHandleCorrectly()
        {
            testAccount.putt = "line_endings_test.txt";
            File.WriteAllText(testAccount.putt, "Номер счёта: 1111\r\nФИО владельца: Тест\r\nБаланс: 1000 р.");
            
            testAccount.num = "2222";
            testAccount.name = "Новое имя";
            testAccount.sum = 2000;
            
            testAccount.pereschet();
            
            var fileContent = File.ReadAllText("line_endings_test.txt");
            StringAssert.Contains("2222", fileContent);
            File.Delete("line_endings_test.txt");
        }

        [Test]
        public void Otk_WithTrailingSpacesInName_ShouldTrim()
        {
            Console.SetIn(new StringReader("  Иванов Иван  \n2000\n"));
            
            testAccount.otk();
            
            Assert.AreEqual("Иванов Иван", testAccount.name);
        }

        [Test]
        public void TopUp_WithMultipleDecimalPlaces_ShouldRoundCorrectly()
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("123.456789\n"));
            
            testAccount.top_up();
            
            Assert.AreEqual(1123.4568f, testAccount.sum, 0.0001f);
        }
        
        
        

        [TearDown]
        public void Cleanup()
        {
            consoleOutput.Dispose();
        }
    }
}