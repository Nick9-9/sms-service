﻿using AutoMapper;
using BAL.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using Model.ViewModels.OperatorViewModels;
using Moq;
using System.Diagnostics;

namespace BAL.Test.ManagersTests
{
	[TestClass]
	public class OperatorManagerTests
	{
		private static Mock<IUnitOfWork> mockUnitOfWork = new Mock<IUnitOfWork>();
		private static Mock<IMapper> mockMapper = new Mock<IMapper>();
		OperatorManager manager = new OperatorManager(mockUnitOfWork.Object, mockMapper.Object);
		public TestContext TestContext { get; set; }

		[TestInitialize]
		public void Initialize()
		{
			TestContext.WriteLine("Initialize test data");
		}

		[TestCleanup]
		public void Cleanup()
		{
			TestContext.WriteLine($"Test name: {TestContext.TestName}");
			TestContext.WriteLine($"Test result: {TestContext.CurrentTestOutcome}");
			TestContext.WriteLine("Cleanup test data");
		}

		[TestMethod]
		public void Add_OperatorObject_SuccessResult()
		{
			#region Arrange
			//задаємо змінні і результат
			OperatorViewModel testOperator = new OperatorViewModel() { Name = "Operator"};
			#endregion

			#region Act
			//запускаємо методи, які тестуються
			var result = manager.Add(testOperator);
			mockUnitOfWork.Verify(f => f.Operators);
			mockUnitOfWork.Verify(f => f.Save());
			#endregion

			#region Assert
			//перевіряємо результат
			Assert.IsTrue(result.Success, result.Details);
			#endregion
		}
	}
}
