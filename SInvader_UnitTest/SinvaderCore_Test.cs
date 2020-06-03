using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SInvader_Core;
using SInvader_Core.i8080;
using SInvader_Core.MMU;

namespace SInvader_UnitTest
{
    [TestClass]
    public class SinvaderCore_Test
    {
        static CPU _cpu = null;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _cpu = new CPU();
            _cpu.Initialize();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _cpu.PC = 0x0;

            _cpu._registers.a = 0x00;
            _cpu._registers.a = 0x02;
            _cpu._registers.HL = 0x00;
            _cpu._registers.BC = 0x00;
            _cpu._registers.DE = 0x00;
        }

        [TestMethod]
        public void DAD_B_0x09()
        {
            _cpu._registers.HL = 0xA17B;
            _cpu._registers.BC = 0x339f;

            Instruction instruction = _cpu.Instructions[0x09];
            _cpu.Instructions[0x09].action(instruction);

            Assert.AreEqual(0xD51A, _cpu._registers.HL);
        }

        [TestMethod]
        public void DAD_D_0x19()
        {
            _cpu._registers.HL = 0xA17B;
            _cpu._registers.DE = 0x339f;

            Instruction instruction = _cpu.Instructions[0x19];
            _cpu.Instructions[0x09].action(instruction);

            Assert.AreEqual(0xD51A, _cpu._registers.HL);
        }

        [TestMethod]
        public void DAD_D_0x29()
        {
            _cpu._registers.HL = 0x339f;

            Instruction instruction = _cpu.Instructions[0x29];
            _cpu.Instructions[0x09].action(instruction);

            Assert.AreEqual(0x673E, _cpu._registers.HL);
        }

        [TestMethod]
        public void DAD_D_0x39()
        {
            _cpu.SP = 0xA17B;
            _cpu._registers.HL = 0x339f;

            Instruction instruction = _cpu.Instructions[0x39];
            _cpu.Instructions[0x09].action(instruction);

            Assert.AreEqual(0xD51A, _cpu._registers.HL);
        }

        [TestMethod]
        public void ADI_d8_0xC6()
        {
            byte[] data = { 0x3E, 0x14, 0xC6, 0x42, 0xC6, 0xBE };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 3; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x14, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 0);
            Assert.AreEqual(_cpu.SFlag, 0);
            Assert.AreEqual(_cpu.PFlag, 1);
            Assert.AreEqual(_cpu.CYFlag, 1);
            Assert.AreEqual(_cpu.ACFlag, 1);
        }

        [TestMethod]
        public void ADI_d8_0xC6_with_subtraction()
        {
            byte[] data = { 0x3E, 0x0C, 0xC6, 0xF1 };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 2; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0xFD, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 0);
            Assert.AreEqual(_cpu.SFlag, 1);
            Assert.AreEqual(_cpu.PFlag, 0);
            Assert.AreEqual(_cpu.CYFlag, 0);
            Assert.AreEqual(_cpu.ACFlag, 0);
        }


        [TestMethod]
        public void ACI_d8_0xCE()
        {
            byte[] data = { 0x3E, 0x56, 0xCE, 0xBE, 0xCE, 0x42 };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 3; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x57, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 0);
            Assert.AreEqual(_cpu.SFlag, 0);
            Assert.AreEqual(_cpu.PFlag, 0);
            Assert.AreEqual(_cpu.CYFlag, 0);
            Assert.AreEqual(_cpu.ACFlag, 0);
        }

        [TestMethod]
        public void SUB_A()
        {
            byte[] data = { 0x3E, 0x3E, 0x97 };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 2; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x0, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 1);
            Assert.AreEqual(_cpu.SFlag, 0);
            Assert.AreEqual(_cpu.PFlag, 1);
            Assert.AreEqual(_cpu.CYFlag, 0);
            Assert.AreEqual(_cpu.ACFlag, 1);
        }

        [TestMethod]
        public void SUI_d8_0xD6()
        {
            byte[] data = { 0x3E, 0x00, 0xD6, 0x01 };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 2; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0xFF, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 0);
            Assert.AreEqual(_cpu.SFlag, 1);
            Assert.AreEqual(_cpu.PFlag, 1);
            Assert.AreEqual(_cpu.CYFlag, 1);
            Assert.AreEqual(_cpu.ACFlag, 0);
        }

        [TestMethod]
        public void SBI_d8_0xDE_WithoutCarry()
        {
            byte[] data = { 0xAF, 0xDE, 0x01 };

            _cpu.CYFlag = 0;
            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 2; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0xFF, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 0);
            Assert.AreEqual(_cpu.SFlag, 1);
            Assert.AreEqual(_cpu.PFlag, 1);
            Assert.AreEqual(_cpu.CYFlag, 1);
            Assert.AreEqual(_cpu.ACFlag, 0);
        }

        [TestMethod]
        public void SBI_d8_0xDE_WithCarry()
        {
            byte[] data = { 0x3E, 0x00, 0xDE, 0x01 };

            _cpu.CYFlag = 1;
            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 2; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0xFE, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 0);
            Assert.AreEqual(_cpu.SFlag, 1);
            Assert.AreEqual(_cpu.PFlag, 0);
            Assert.AreEqual(_cpu.CYFlag, 1);
            Assert.AreEqual(_cpu.ACFlag, 0);
        }

        [TestMethod]
        public void ANI_d8_0xE6()
        {
            byte[] data = { 0x3E, 0x3A, 0xE6, 0x0F };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 2; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x0A, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 0);
            Assert.AreEqual(_cpu.SFlag, 0);
            Assert.AreEqual(_cpu.PFlag, 1);
            Assert.AreEqual(_cpu.CYFlag, 0);
            Assert.AreEqual(_cpu.ACFlag, 0);
        }

        [TestMethod]
        public void XOR_d8_0xEE()
        {
            byte[] data = { 0x3E, 0x3B, 0xEE, 0x81 };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 2; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0xBA, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 0);
            Assert.AreEqual(_cpu.SFlag, 1);
            Assert.AreEqual(_cpu.PFlag, 0);
            Assert.AreEqual(_cpu.CYFlag, 0);
            Assert.AreEqual(_cpu.ACFlag, 0);
        }

        [TestMethod]
        public void ORI_d8_0xF6()
        {
            byte[] data = { 0x3E, 0xB5, 0xF6, 0x0F };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 2; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0xBF, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 0);
            Assert.AreEqual(_cpu.SFlag, 1);
            Assert.AreEqual(_cpu.PFlag, 0);
            Assert.AreEqual(_cpu.CYFlag, 0);
            Assert.AreEqual(_cpu.ACFlag, 0);
        }

        [TestMethod]
        public void CPI_d8_0xFE()
        {
            byte[] data = { 0x3E, 0x4A, 0xFE, 0x40 };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 2; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x4A, _cpu._registers.a);
            Assert.AreEqual(_cpu.ZFlag, 0);
            Assert.AreEqual(_cpu.SFlag, 0);
            Assert.AreEqual(_cpu.PFlag, 1);
            Assert.AreEqual(_cpu.CYFlag, 0);
            Assert.AreEqual(_cpu.ACFlag, 1);
        }

        [TestMethod]
        public void INX_B_0x03()
        {
            byte[] data = { 0x3E, 0x38, 0x47, 0x3E, 0xFF, 0x4F, 0x03 };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 5; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x3900, _cpu._registers.BC);
        }

        [TestMethod]
        public void INX_D_0x13()
        {
            byte[] data = { 0x3E, 0x38, 0x57, 0x3E, 0xFF, 0x5F, 0x13 };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 5; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x3900, _cpu._registers.DE);
        }

        [TestMethod]
        public void INX_H_0x23()
        {
            byte[] data = { 0x3E, 0x38, 0x67, 0x3E, 0xFF, 0x6F, 0x23 };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 5; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x3900, _cpu._registers.HL);
        }

        [TestMethod]
        public void INX_SP_0x33()
        {
            byte[] data = { 0x33 };

            _cpu.SP = 0xFFFF;
            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 1; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x0000, _cpu.SP);
        }

        [TestMethod]
        public void DCX_B_0x0B()
        {
            byte[] data = { 0x3E, 0x98, 0x47, 0x3E, 0x00, 0x4F, 0x0B };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 5; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x97FF, _cpu._registers.BC);
        }

        [TestMethod]
        public void DCX_D_0x1B()
        {
            byte[] data = { 0x3E, 0x98, 0x57, 0x3E, 0x00, 0x5F, 0x1B };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 5; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x97FF, _cpu._registers.DE);
        }

        [TestMethod]
        public void DCX_H_0x2B()
        {
            byte[] data = { 0x3E, 0x98, 0x67, 0x3E, 0x00, 0x6F, 0x2B };

            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 5; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0x97FF, _cpu._registers.HL);
        }

        [TestMethod]
        public void DCX_SP_0x33()
        {
            byte[] data = { 0x3B };

            _cpu.SP = 0xFFFF;
            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 1; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0xFFFE, _cpu.SP);
        }

        [TestMethod]
        public void LDAX_B()
        {
            byte[] data = { 0x06, 0x93, 0x0E, 0x8B, 0x0A };

            _cpu.MCU.WriteByte(0x938b, 0xFE);
            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 3; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0xFE, _cpu._registers.a);
        }

        [TestMethod]
        public void LDAX_D()
        {
            byte[] data = { 0x16, 0x93, 0x1E, 0x8B, 0x1A };

            _cpu.MCU.WriteByte(0x938b, 0xFF);
            _cpu.MCU.LoadDataInMemoyAt(data, 0x0);

            for (int i = 0; i < 3; i++)
            {
                _cpu.Fetch();
                _cpu.Execute();
            }

            Assert.AreEqual(0xFF, _cpu._registers.a);
        }
    }
}
