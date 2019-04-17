﻿using AutoMapper;
using BAL.Managers;
using Model.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Diagnostics;
using WebApp.Models;
using System.Linq;
using System;
using System.Linq.Expressions;
using BAL.Interfaces;
using Model.DTOs;
using Model.ViewModels.RecievedMessageViewModel;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace BAL.Tests.ManagersTests
{
    public class RecievedMessageManagerTests : TestInitializer
    {
        IRecievedMessageManager recievedMessageManager ;

        private RecievedMessage message;
        private RecievedMessageViewModel viewMessage;
        private Phone phoneSender;
        private Phone phoneRecipient;
        private List<Phone> listPhones;
        private SubscribeWord subscribeWord;
        private List<SubscribeWord> listSubscribeWords;
        private RecievedMessageDTO recievedMessageDto;
        private RecievedMessageDTO recievedMessageInsert;
        private StopWord stopWord;
        private List<StopWord> listStopWords;
        private Company testCompany;
        private List<Company> listCompanies;
        [SetUp]
        public void SetUp()
        {
            base.Initialize();
            recievedMessageManager = new RecievedMessageManager(mockUnitOfWork.Object, mockMapper.Object);
            TestContext.WriteLine("Overrided");
            phoneSender =new Phone(){Id=9,PhoneNumber = "+380999999999" };
            phoneRecipient = new Phone() { Id = 10, PhoneNumber = "+380111111111" };
            listPhones = new List<Phone>()
            {
                phoneSender,
                phoneRecipient,
                new Phone() {Id = 7,PhoneNumber = "+380501465619"}
            };

            testCompany =new Company()
            {   Id=1,
                PhoneId = 10,
                Name= "Test",
                ApplicationGroupId = 5

            };
            listCompanies=new List<Company>(){ testCompany };
            subscribeWord =new SubscribeWord(){Id = 21,CompanyId = 1,SubscribePhoneId =10,Word = "sWord"};

            listSubscribeWords=new List<SubscribeWord>()
            {
                subscribeWord,
                new SubscribeWord() { Id = 2, Word = "test2" }
            };
            message = new RecievedMessage() {Id = 3,CompanyId = testCompany.Id, PhoneId = 10,Message = "test"};
            viewMessage= new RecievedMessageViewModel()
            {
                SenderPhone = "+380999999999",
                RecipientPhone = "+380111111111",
                MessageText = "sWord"
            };
            recievedMessageInsert=new RecievedMessageDTO()
            {
                SenderPhone = viewMessage.SenderPhone,
                RecipientPhone = viewMessage.RecipientPhone,
                MessageText = "test1"
            };
            recievedMessageDto =new RecievedMessageDTO()
           {
               SenderPhone = viewMessage.SenderPhone,
               RecipientPhone = viewMessage.RecipientPhone,
               MessageText = viewMessage.MessageText

           };
            stopWord = new StopWord() {Id = 1, Word = "sWord"};
            listStopWords=new List<StopWord>()
            {
                stopWord

            };
        }


    [Test]
        public void Get_SuccessResult()
        {
            mockUnitOfWork.Setup(n => n.RecievedMessages.GetById(3)).Returns( message );

            mockMapper.Setup(m => m.Map<RecievedMessage, RecievedMessageViewModel>(message)).Returns(viewMessage);


            var result = recievedMessageManager.Get(3);
            Assert.That(result,Is.EqualTo(viewMessage));
        }

        [Test]
        public void Delete_SuccessResult()
        {
            mockUnitOfWork.Setup(m => m.RecievedMessages.GetById(3)).Returns(message);
            mockUnitOfWork.Setup(m => m.RecievedMessages.Delete(message));
            mockUnitOfWork.Setup(m => m.Save());

            var result = recievedMessageManager.Delete(3);
            Assert.IsTrue(result);
        }

        [Test]
        public void Delete_ThrowExceptionResult()
        {
           mockUnitOfWork.Setup(m => m.RecievedMessages.GetById(2)).Returns(message);
            mockUnitOfWork.Setup(m => m.RecievedMessages.Delete(message)).Throws(new Exception());
            mockUnitOfWork.Setup(m => m.Save());

            var result = recievedMessageManager.Delete(2);
            Assert.IsFalse(result);
        }

        #region RecievedMessageManager

        [Test]
        public void SSubscribeWordInM_RecivedMessage_NoSuchWord()
        {
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                 .Returns(new List<SubscribeWord>() { new SubscribeWord() { Id = 2, Word = "test1" } });
            var result = recievedMessageManager.SearchSubscribeWordInMessages(recievedMessageDto);

            Assert.IsFalse(result);
        }

        [Test]
        [TestCase]
        public void SSubscribeWordInM_RecivedMessage_NullOrignator()
        {
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                .Returns(listSubscribeWords);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(new List<Phone>(){phoneRecipient,new Phone(){Id=2,PhoneNumber = "+380550505055"}});

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);

            mockUnitOfWork.Setup(m => m.Recipients.GetAll()).Returns(new List<Recipient>());

            mockUnitOfWork.Setup(m => m.Recipients.Insert(new Recipient()));

            var result = recievedMessageManager.SearchSubscribeWordInMessages(recievedMessageDto);

            Assert.IsTrue(result);
        }

        [Test]
        public void SSubscribeWordInM_RecivedMessage_NullSubscribePhoneId()
        {
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                .Returns(new List<SubscribeWord>() { new SubscribeWord(){ Id = 21, CompanyId = 1, SubscribePhoneId = null, Word = "subWord" } });

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(new List<Company>() { testCompany });

            mockUnitOfWork.Setup(m => m.Recipients.GetAll()).Returns(new List<Recipient>());

            mockUnitOfWork.Setup(m => m.Recipients.Insert(new Recipient()));

            var result = recievedMessageManager.SearchSubscribeWordInMessages(recievedMessageDto);

            Assert.IsFalse(result);
        }
        [Test]
        public void SSubscribeWordInM_RecivedMessage_CompanyPhoneEqualRecipientPhone()
        {
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                .Returns(listSubscribeWords);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Phones.GetById((int) subscribeWord.SubscribePhoneId)).Returns(phoneRecipient);
            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(new List<Company>() { testCompany });

            mockUnitOfWork.Setup(m => m.Recipients.GetAll()).Returns(new List<Recipient>());

            mockUnitOfWork.Setup(m => m.Recipients.Insert(new Recipient()));


            var recivedMessDto = new RecievedMessageDTO()
            {
                SenderPhone = viewMessage.RecipientPhone,
                RecipientPhone = viewMessage.RecipientPhone,
                MessageText = viewMessage.MessageText
            };
            var result = recievedMessageManager.SearchSubscribeWordInMessages(recivedMessDto);

            Assert.IsFalse(result);
        }
        [Test]
        public void SSubscribeWordInM_RecivedMessage_NullCompanyException()
        {
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                .Returns(listSubscribeWords);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns((List<Company>)null);

            mockUnitOfWork.Setup(m => m.Recipients.GetAll()).Returns(new List<Recipient>());

            mockUnitOfWork.Setup(m => m.Recipients.Insert(new Recipient()));

            var result = recievedMessageManager.SearchSubscribeWordInMessages(recievedMessageDto);

            Assert.IsFalse(result);
        }

        [Test]
        public void SSubscribeWordInM_RecivedMessage_NullCompany()
        {
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                .Returns(listSubscribeWords);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(new List<Company>(){new Company(){Id=3,PhoneId = 999}});

            mockUnitOfWork.Setup(m => m.Recipients.GetAll()).Returns(new List<Recipient>());

            mockUnitOfWork.Setup(m => m.Recipients.Insert(new Recipient()));

            var result = recievedMessageManager.SearchSubscribeWordInMessages(recievedMessageDto);

            Assert.IsFalse(result);
        }

        [Test]
        public void SSubscribeWordInM_RecivedMessage_SuccessResult()
        {
            mockUnitOfWork.Setup(m=>m.SubscribeWords.GetAll())
                .Returns(listSubscribeWords);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(new List<Company>(){testCompany});

            mockUnitOfWork.Setup(m => m.Recipients.GetAll()).Returns(new List<Recipient>());

            mockUnitOfWork.Setup(m => m.Recipients.Insert(new Recipient()));

            var result = recievedMessageManager.SearchSubscribeWordInMessages(recievedMessageDto);

            Assert.IsTrue(result);
        }
        #endregion

        #region SearchStopWordInMessages

        [Test]
        public void SStopWordInM_RecivedMessage_NullStopWord()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(new List<StopWord>() { new StopWord() { Id = 2, Word = "test1" } });

            var result = recievedMessageManager.SearchStopWordInMessages(recievedMessageDto);

            Assert.IsFalse(result);
        }
        [Test]
        public void SStopWordInM_RecivedMessage_ExceptionNullCompanyObject()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(listStopWords);
            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns((List<Company>)null);

            var result = recievedMessageManager.SearchStopWordInMessages(recievedMessageDto);

            Assert.IsFalse(result);
        }
        [Test]
        public void SStopWordInM_RecivedMessage_ExcenNullObject()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(listStopWords);
            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(new List<Phone>() { phoneRecipient, new Phone() { Id = 2, PhoneNumber = "+380550505055" } });

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);

            var result = recievedMessageManager.SearchStopWordInMessages(recievedMessageDto);

            Assert.IsFalse(result);
        }
        [Test]
        public void SStopWordInM_RecivedMessage_NullPhoneGroupUnsubscribe()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(new List<StopWord>(){new StopWord() {Id = 3,Word = "START"}});
            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);
            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.GetAll())
                .Returns((List<PhoneGroupUnsubscribe>) null);
            var result = recievedMessageManager.SearchStopWordInMessages(recievedMessageDto);

            Assert.IsFalse(result);
        }
        [Test]
        public void SStopWordInM_RecivedMessage_PhoneGroupUnsubscribeNull()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(new List<StopWord>() { new StopWord() { Id = 3, Word = "START" } });
            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);
            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.GetAll())
                .Returns((new List<PhoneGroupUnsubscribe>()));
            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.Delete(new PhoneGroupUnsubscribe()));
            mockUnitOfWork.Setup(m => m.Save());

            var result = recievedMessageManager.SearchStopWordInMessages(recievedMessageDto);

            Assert.IsFalse(result);
        }
        [Test]
        public void SStopWordInM_RecivedMessage_PhoneGroupUnsubscribe()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(new List<StopWord>() { new StopWord() { Id = 3, Word = "START" } });
            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);
            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.GetAll())
                .Returns((new List<PhoneGroupUnsubscribe>(){new PhoneGroupUnsubscribe(){GroupId = testCompany.ApplicationGroupId,PhoneId = phoneSender.Id}}));
            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.Delete(new PhoneGroupUnsubscribe()));
            mockUnitOfWork.Setup(m => m.Save());

            var result = recievedMessageManager.SearchStopWordInMessages(recievedMessageDto);

            Assert.IsTrue(result);
        }

        [Test]
        public void SStopWordInM_RecivedMessage_SuccessResult()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(listStopWords);
            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);
        
            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.Insert(new PhoneGroupUnsubscribe()));
            mockUnitOfWork.Setup(m => m.Save());

            var result = recievedMessageManager.SearchStopWordInMessages(recievedMessageDto);

            Assert.IsTrue(result);
        }
        #endregion


        [Test]
        public void Insert_RecivedMessage_SuccessResult()
        {
            mockUnitOfWork.Setup(m => m.Phones.Insert(new Phone()));
            mockUnitOfWork.Setup(m => m.RecievedMessages.Insert(message));
            mockUnitOfWork.Setup(m => m.Save());
            mockMapper.Setup(m => m.Map<RecievedMessageDTO, RecievedMessage>(recievedMessageInsert)).Returns(message);
               
         
            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(new List<StopWord>(){new StopWord(){Id = 1234,Word = "1234"}});
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                .Returns(new List<SubscribeWord>() { new SubscribeWord() { Id = 1234, Word = "1234" } });

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);
            var result = recievedMessageManager.Insert(recievedMessageInsert);

            Assert.IsTrue(result);
        }
    }
}