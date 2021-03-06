﻿using AutoMapper;
using BAL.Managers;
using Model.Interfaces;
using Moq;
using BAL.Exceptions;
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
            subscribeWord =new SubscribeWord(){Id = 21,Word = "sWord"};

            listSubscribeWords=new List<SubscribeWord>()
            {
                subscribeWord,
                new SubscribeWord() { Id = 2, Word = "test2" }
            };
            message = new RecievedMessage() {Id = 3,CompanyId = testCompany.Id, PhoneId = phoneRecipient.Id, Message = "test"};
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
                stopWord,
                new StopWord(){Id=2,Word = "START"}
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

            Assert.That(() => { recievedMessageManager.Delete(2); }, Throws.Nothing);
        }

     

        #region RecievedMessageManager

        [Test]
        public void SSubscribeWordInM_SubscribeWordNull_Nothing()
        {
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                 .Returns(new List<SubscribeWord>() { new SubscribeWord() { Id = 2, Word = "test1" } });
            Assert.That(() => { recievedMessageManager.SearchSubscribeWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }

        [Test]
        public void SSubscribeWordInM_NullOrignator_NothingSuccessResult()
        {
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                .Returns(listSubscribeWords);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(new List<Phone>(){phoneRecipient,new Phone(){Id=2,PhoneNumber = "+380550505055"}});

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);
            mockUnitOfWork.Setup(m =>
                    m.CompanySubscribeWords.Get(It.IsAny<Expression<Func<CompanySubscribeWord, bool>>>(), null, ""))
                .Returns(new List<CompanySubscribeWord>(){new CompanySubscribeWord(){CompanyId =testCompany.Id,SubscribeWordId = subscribeWord.Id } });
            mockUnitOfWork.Setup(m => m.Recipients.GetAll()).Returns(new List<Recipient>());

            mockUnitOfWork.Setup(m => m.Recipients.Insert(new Recipient()));

            Assert.That(() => { recievedMessageManager.SearchSubscribeWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }

     
        [Test]
        public void SSubscribeWordInM_CompanySubscribeWordsEqualToNull_Nothing()
        {
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                .Returns(listSubscribeWords);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(new List<Company>() { testCompany });
            mockUnitOfWork.Setup(m =>
                    m.CompanySubscribeWords.Get(It.IsAny<Expression<Func<CompanySubscribeWord, bool>>>(), null, ""))
                .Returns(new List<CompanySubscribeWord>());
            mockUnitOfWork.Setup(m => m.Recipients.GetAll()).Returns(new List<Recipient>());

            mockUnitOfWork.Setup(m => m.Recipients.Insert(new Recipient()));


            var recivedMessDto = new RecievedMessageDTO()
            {
                SenderPhone = viewMessage.RecipientPhone,
                RecipientPhone = viewMessage.RecipientPhone,
                MessageText = viewMessage.MessageText
            };
            Assert.That(() => { recievedMessageManager.SearchSubscribeWordInMessages(recivedMessDto); }, Throws.Nothing);
        }
        [Test]
        public void SSubscribeWordInM_DestinationNull_Nothing()
        {
            mockUnitOfWork.Setup(m => m.SubscribeWords.GetAll())
                .Returns(listSubscribeWords);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(new List<Phone>(){phoneSender});

            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(new List<Phone>() { phoneSender });

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(new List<Company>() { testCompany });
           
            mockUnitOfWork.Setup(m => m.Recipients.GetAll()).Returns(new List<Recipient>() { new Recipient() { CompanyId = testCompany.Id, PhoneId = phoneSender.Id } });
            mockUnitOfWork.Setup(m =>
                    m.CompanySubscribeWords.Get(It.IsAny<Expression<Func<CompanySubscribeWord, bool>>>(), null, ""))
                .Returns(new List<CompanySubscribeWord>() { new CompanySubscribeWord() { CompanyId = testCompany.Id, SubscribeWordId = subscribeWord.Id } });

            mockUnitOfWork.Setup(m => m.Recipients.Insert(new Recipient()));

            Assert.That(() => { recievedMessageManager.SearchSubscribeWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }

        [Test]
        public void SSubscribeWordInM_SuccessResult_Nothing()
        {
            mockUnitOfWork.Setup(m=>m.SubscribeWords.GetAll())
                .Returns(listSubscribeWords);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(new List<Company>(){testCompany});
           // r.CompanyId == subscribeCompany.Id && r.PhoneId == orignator.Id
            mockUnitOfWork.Setup(m => m.Recipients.GetAll()).Returns(new List<Recipient>(){new Recipient(){CompanyId = testCompany.Id,PhoneId = phoneSender.Id}});
            mockUnitOfWork.Setup(m =>
                    m.CompanySubscribeWords.Get(It.IsAny<Expression<Func<CompanySubscribeWord, bool>>>(), null, ""))
                .Returns(new List<CompanySubscribeWord>() { new CompanySubscribeWord() { CompanyId = testCompany.Id, SubscribeWordId = subscribeWord.Id } });

            mockUnitOfWork.Setup(m => m.Recipients.Insert(new Recipient()));

          Assert.That(() => { recievedMessageManager.SearchSubscribeWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }
        #endregion
        
        #region SearchStopWordInMessages

        [Test]
        public void SStopWordInM_NullStopWord_Nothing()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(new List<StopWord>() { new StopWord() { Id = 2, Word = "test1" } });

            Assert.That(() => { recievedMessageManager.SearchStopWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }
        [Test]
        public void SStopWordInM_ExceptionNullCompanyObject_Nothing()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(listStopWords);
            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns((List<Company>)null);

            Assert.That(() => { recievedMessageManager.SearchStopWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }
        [Test]
        public void SStopWordInM_ExcenNullObject_Nothing()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(listStopWords);
            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(new List<Phone>() { phoneRecipient, new Phone() { Id = 2, PhoneNumber = "+380550505055" } });

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);

            Assert.That(() => { recievedMessageManager.SearchStopWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }
        [Test]
        public void SStopWordInM_NullPhoneGroupUnsubscribe_Nothing()
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

            Assert.That(() => { recievedMessageManager.SearchStopWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }
        [Test]
        public void SStopWordInM_StopWordStart_PhoneGroupUnsubscribeNull_Nothing()
        {
        mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(new List<StopWord>() { new StopWord() { Id = 3, Word = "START" } });
            
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);

            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.GetAll())
                .Returns((new List<PhoneGroupUnsubscribe>()));

            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.Delete(new PhoneGroupUnsubscribe()));
           mockUnitOfWork.Setup(m => m.Save());
           recievedMessageDto.MessageText = "START";
            Assert.That(() => { recievedMessageManager.SearchStopWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }
        [Test]
        public void SStopWordInM_PhoneGroupUnsubscribe_Nothing()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(new List<StopWord>() { new StopWord() { Id = 3, Word = "START" } });
           
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);
            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.GetAll())
                .Returns((new List<PhoneGroupUnsubscribe>(){new PhoneGroupUnsubscribe(){GroupId = testCompany.ApplicationGroupId,PhoneId = phoneSender.Id}}));

            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.Delete(new PhoneGroupUnsubscribe()));
            mockUnitOfWork.Setup(m => m.Save());
            recievedMessageDto.MessageText = "START";
            Assert.That(() => { recievedMessageManager.SearchStopWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }

        [Test]
        public void SStopWordInM_SuccessResult_Nothing()
        {
            mockUnitOfWork.Setup(m => m.StopWords.GetAll())
                .Returns(listStopWords);
           
            mockUnitOfWork.Setup(m => m.Phones.GetAll())
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);
        
            mockUnitOfWork.Setup(m => m.PhoneGroupUnsubscribes.Insert(new PhoneGroupUnsubscribe()));
            mockUnitOfWork.Setup(m => m.Save());

            Assert.That(() => { recievedMessageManager.SearchStopWordInMessages(recievedMessageDto); }, Throws.Nothing);
        }
        #endregion
        [Test]
        public void Insert_PhoneNull_Nothing()
        {
            mockUnitOfWork.Setup(m => m.Phones.Insert(new Phone()));
            mockUnitOfWork.Setup(m => m.RecievedMessages.Insert(message));
            mockUnitOfWork.Setup(m => m.Save());
            mockMapper.Setup(m => m.Map<RecievedMessageDTO, RecievedMessage>(recievedMessageInsert)).Returns(message);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(new List<Phone>());

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(listCompanies);

            Assert.That(() => { recievedMessageManager.Insert(recievedMessageInsert); }, Throws.Nothing);
        }
        [Test]
        public void Insert_CompanyNull_Nothing()
        {
            mockUnitOfWork.Setup(m => m.Phones.Insert(new Phone()));
            mockUnitOfWork.Setup(m => m.RecievedMessages.Insert(message));
            mockUnitOfWork.Setup(m => m.Save());
            mockMapper.Setup(m => m.Map<RecievedMessageDTO, RecievedMessage>(recievedMessageInsert)).Returns(message);

            mockUnitOfWork.Setup(m => m.Phones.Get(It.IsAny<Expression<Func<Phone, bool>>>(), null, ""))
                .Returns(listPhones);

            mockUnitOfWork.Setup(m => m.Companies.Get(It.IsAny<Expression<Func<Company, bool>>>(), null, ""))
                .Returns(new List<Company>());

            Assert.That(() => { recievedMessageManager.Insert(recievedMessageInsert); }, Throws.Nothing);
        }
        [Test]
        public void Insert_SuccessResult_Nothing()
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
         
           Assert.That(() => { recievedMessageManager.Insert(recievedMessageInsert); }, Throws.Nothing);
        }

        [Test]
        public void GetRecievedMessages_SuccessResult_Nothing()
        {
            mockUnitOfWork.Setup(m =>
                m.RecievedMessages.Get(It.IsAny<Expression<Func<RecievedMessage, bool>>>(), null, ""))
                .Returns(new List<RecievedMessage>(){message});
            mockUnitOfWork.Setup(m=>m.Companies.GetById(message.CompanyId)).Returns(testCompany);
            mockUnitOfWork.Setup(m => m.Phones.GetById(message.CompanyId)).Returns(phoneRecipient);
            mockUnitOfWork.Setup(m => m.Phones.GetById(message.PhoneId)).Returns(phoneSender);
            mockMapper.Setup(m => m.Map<RecievedMessage, RecievedMessageViewModel>(message)).Returns(viewMessage);

            var result = recievedMessageManager.GetRecievedMessages(message.CompanyId);
            Assert.That(result,Is.EqualTo(new List<RecievedMessageViewModel>()));
        }
        [Test]
        public void GetRecievedMessages_NullPhoneIdSuccessResult_Nothing()
        {
            mockUnitOfWork.Setup(m =>
                    m.RecievedMessages.Get(It.IsAny<Expression<Func<RecievedMessage, bool>>>(), null, ""))
                .Returns(new List<RecievedMessage>() { message });
            mockUnitOfWork.Setup(m => m.Companies.GetById(message.CompanyId))
                .Returns(new Company(){Name = testCompany.Name,
                    PhoneId = null,
                    Id = 1,
                    ApplicationGroupId = testCompany.ApplicationGroupId
                });
            mockUnitOfWork.Setup(m => m.Phones.GetById(message.CompanyId)).Returns(phoneRecipient);
            mockUnitOfWork.Setup(m => m.Phones.GetById(message.PhoneId)).Returns(phoneSender);
            mockMapper.Setup(m => m.Map<RecievedMessage, RecievedMessageViewModel>(message)).Returns(viewMessage);

            var result = recievedMessageManager.GetRecievedMessages(message.CompanyId);
            Assert.That(result, Is.EqualTo(new List<RecievedMessageViewModel>()));
        }
   
    }
}
