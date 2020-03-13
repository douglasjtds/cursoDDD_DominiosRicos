using System;
using Flunt.Notifications;
using Flunt.Validations;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : Notifiable, 
            IHandler<CreateBoletoSubscriptionCommand>, 
            IHandler<CreatePayPalSubscriptionCommand>, 
            IHandler<CreateCreditCardSubscriptionCommand>
    {
        private readonly IStudentRepository studentRepository;
        private readonly IEmailService emailService;

        public SubscriptionHandler(IStudentRepository studentRepository, IEmailService emailService) //injeção de dependência
        {
            this.studentRepository = studentRepository;
            this.emailService = emailService;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            //Fail Fast Validation
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível realizar seu cadastro.");
            }

            //verificar se Documento já está cadastrado
            if (studentRepository.DocumentExists(command.Document))
                AddNotification("Document", "Este CPF já está em uso.");

            //verificar se E-mail já está cadastrado
            if (studentRepository.EmailExists(command.Email))
                AddNotification("Email", "Este E-mail já está em uso.");

            //Gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            //Gerar as Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(command.BarCode, command.BoletoNumber, command.PaidDate,
                            command.ExpireDate, command.Total, command.TotalPaid, command.Payer,
                            new Document(command.PayerDocument, command.PayerDocumentType), address, email);

            //Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubcription(subscription);


            //Agrupar as Validações
            AddNotifications(name, document, email, address, student, subscription, payment);

            //Salvar as informações
            studentRepository.CreateSubscription(student);

            //Enviar E-mail de boas vindas
            emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao balta.io", "Sua assinatura foi criada");

            //Retornar informações
            return new CommandResult(true, "Assinatura realizada com sucesso.");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            // //Fail Fast Validation
            // command.Validate();
            // if (command.Invalid)
            // {
            //     AddNotifications(command);
            //     return new CommandResult(false, "Não foi possível realizar seu cadastro.");
            // }

            //verificar se Documento já está cadastrado
            if (studentRepository.DocumentExists(command.Document))
                AddNotification("Document", "Este CPF já está em uso.");

            //verificar se E-mail já está cadastrado
            if (studentRepository.EmailExists(command.Email))
                AddNotification("Email", "Este E-mail já está em uso.");

            //Gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            //Gerar as Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(command.TransactionCode, command.PaidDate,
                            command.ExpireDate, command.Total, command.TotalPaid, command.Payer,
                            new Document(command.PayerDocument, command.PayerDocumentType), address, email);

            //Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubcription(subscription);


            //Agrupar as Validações
            AddNotifications(name, document, email, address, student, subscription, payment);

            //Salvar as informações
            studentRepository.CreateSubscription(student);

            //Enviar E-mail de boas vindas
            emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao balta.io", "Sua assinatura foi criada");

            //Retornar informações
            return new CommandResult(true, "Assinatura realizada com sucesso.");
        }

        public ICommandResult Handle(CreateCreditCardSubscriptionCommand command)
        {
            // //Fail Fast Validation
            // command.Validate();
            // if (command.Invalid)
            // {
            //     AddNotifications(command);
            //     return new CommandResult(false, "Não foi possível realizar seu cadastro.");
            // }

            //verificar se Documento já está cadastrado
            if (studentRepository.DocumentExists(command.Document))
                AddNotification("Document", "Este CPF já está em uso.");

            //verificar se E-mail já está cadastrado
            if (studentRepository.EmailExists(command.Email))
                AddNotification("Email", "Este E-mail já está em uso.");

            //Gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            //Gerar as Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new CreditCardPayment(command.CardHolderName, command.CardNumber, command.LastTransactionNumber, command.PaidDate,
                            command.ExpireDate, command.Total, command.TotalPaid, command.Payer,
                            new Document(command.PayerDocument, command.PayerDocumentType), address, email);

            //Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubcription(subscription);


            //Agrupar as Validações
            AddNotifications(name, document, email, address, student, subscription, payment);

            //Salvar as informações
            studentRepository.CreateSubscription(student);

            //Enviar E-mail de boas vindas
            emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao balta.io", "Sua assinatura foi criada");

            //Retornar informações
            return new CommandResult(true, "Assinatura realizada com sucesso.");
        }
    }
}