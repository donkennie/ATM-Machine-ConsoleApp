// See https://aka.ms/new-console-template for more information
using ATMApp.Domain.Entities;
using ATMApp.Domain.Enums;
using ATMApp.Domain.Interfaces;
using ATMApp.UI;


namespace ATMApp
{
    public class ATMApp : IUserLogin, IUserAccountActions, ITransaction
    {
        private List<UserAccount> userAccountList;

        private UserAccount selectedAccount;

        private List<Transaction> _listOfTransactions;

        private const decimal minimumKeptAmount = 500;

      //  private readonly AppScreen screen;


        public void Run()
        {
            AppScreen.Welcome();

            CheckUserCardNumAndPassword();

            AppScreen.WelcomeCustomer(selectedAccount.FullName);

            AppScreen.DisplayAppMenu();

            ProcessMenuoption();
        }

        public void InitializeData()
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount{Id=1, FullName = "Obinna Ezeh", AccountNumber=123456,CardNumber =321321, CardPin=123123,AccountBalance=50000.00m,IsLocked=false},
                new UserAccount{Id=2, FullName = "Amaka Hope", AccountNumber=456789,CardNumber =654654, CardPin=456456,AccountBalance=4000.00m,IsLocked=false},
                new UserAccount{Id=3, FullName = "Femi Sunday", AccountNumber=123555,CardNumber =987987, CardPin=789789,AccountBalance=2000.00m,IsLocked=true},
            };

            _listOfTransactions = new List<Transaction>();
        }


        public void CheckUserCardNumAndPassword()
        {
            bool isCorrectLogin = false;

            while (isCorrectLogin == false)
            {

                UserAccount inputAccount = AppScreen.UserLoginForm();

                AppScreen.LoginProgress();

                foreach (UserAccount account in userAccountList)
                {
                    selectedAccount = account;

                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        selectedAccount.TotalLogin++;

                        if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
                        {
                            selectedAccount = account;

                            if (selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                            {
                                AppScreen.PrintLockScreen();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }

                }

                if (isCorrectLogin == false)
                {
                    Utility.PrintMessage("\nInvalid card number or PIN.", false);
                    selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                    if (selectedAccount.IsLocked)
                    {
                        AppScreen.PrintLockScreen();
                    }
                }

                Console.Clear();


            }

        }

        private void ProcessMenuoption()
        {
            switch (Validator.Convert<int>("an option:"))
            {
                case (int)AppMenu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)AppMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)AppMenu.MakeWithdrawal:
                    MakeWithDrawal();
                    break;
                case (int)AppMenu.InternalTransfer:
                    var internalTransfer = screen.InternalTransferForm();
                    ProcessInternalTransfer(internalTransfer);
                    break;
                case (int)AppMenu.ViewTransaction:
                    ViewTransaction();
                    break;
                case (int)AppMenu.Logout:
                    AppScreen.LogoutProgress();
                    Utility.PrintMessage("You have successfully logged out. Please collect " +
                        "your ATM card.");
                    Run();
                    break;
                default:
                    Utility.PrintMessage("Invalid Option.", false);
                    break;
            }

       }


        public void CheckBalance()
        {
            Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}");
        }


        public void PlaceDeposit()
        {
            Console.WriteLine("\nOnly multiples of 500 and 1000 naira allowed.\n");
            var transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");

            //simulate counting
            Console.WriteLine("\nChecking and Counting bank notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            //some gaurd clause
            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be great er than zero. Try again.", false); ;
                return;
            }
            if (transaction_amt % 500 != 0)
            {
                Utility.PrintMessage($"Enter deposit amount in multiples of 500 or 1000. Try again.", false);
                return;
            } 

            if (PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"You have cancelled your action.", false);
                return;
            }

            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transaction_amt, "");

            //update account balance
            selectedAccount.AccountBalance += transaction_amt;

            //print success message
            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was " +
                $"succesful.", true);



        }


        private bool PreviewBankNotesCount(int amount)
        {

            int thousandNotesCount = amount / 1000;
            int fiveHundredNotesCount = (amount % 1000) / 500;

            Console.WriteLine("\nSummary");
            Console.WriteLine("------");
            Console.WriteLine($"{AppScreen.cur}1000 X {thousandNotesCount} = {1000 * thousandNotesCount}");
            Console.WriteLine($"{AppScreen.cur}500 X {fiveHundredNotesCount} = {500 * fiveHundredNotesCount}");
            Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");

            int opt = Validator.Convert<int>("1 to confirm");
            return opt.Equals(1);

        }


        public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc)
        {
            //create a new transaction object
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Descriprion = _desc
            };

            //add transaction object to the list
            _listOfTransactions.Add(transaction);
        }

        public void ViewTransaction()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();
            //check if there's a transaction
            if (filteredTransactionList.Count <= 0)
            {
                Utility.PrintMessage("You have no transaction yet.", true);
            }
            else
            {
                var table = new ConsoleTable("Id", "Transaction Date", "Type", "Descriptions", "Amount " + AppScreen.cur);
                foreach (var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Descriprion, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)", true);
            }
        }

    }
}
