using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Entities;

namespace PaymentContext.Tests
{
    [TestClass]
    public class StudentTests
    {
        [TestMethod]
        public void AddSubscription()
        {
            var subscription = new Subscription();
            var student = new Student("Douglas", "Santos", "123465", "teste@teste.com.br");
            // student.Subscriptions.Add(subscription);
            student.AddSubcription(subscription);
        }
    }
}