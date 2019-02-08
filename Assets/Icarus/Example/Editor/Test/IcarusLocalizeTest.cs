using Icarus.Core;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Icarus.Editor.Test
{
    public class IcarusLocalizerTest
    {
        [Test]
        public void Test()
        {
            var localize = TextLocalizer.CreateLocalizedModel("ja", "en", $"key,ja,en{Environment.NewLine}KeyTest,テスト{Environment.NewLine}KeyTest2,テスト2,Test2{Environment.NewLine}KeyTest3,テスト3,Test3");
            Assert.AreEqual(localize.GetText("KeyTest"), "テスト");
        }

        /*
        [Test]
        public void ConvertTextToDic()
        {
            TextLocalizer.Initialize("ja", "ja", $"key,ja,en{Environment.NewLine}KeyTest,テスト,Test{Environment.NewLine}");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest"), "テスト");

            TextLocalizer.Initialize("en", "en", $"key,ja,en{Environment.NewLine}KeyTest,テスト,Test");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest"), "Test");

            TextLocalizer.Initialize("en", "en", $"key,ja,en{Environment.NewLine}KeyTest,テスト,Test{Environment.NewLine}");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest"), "Test");
        }

        [Test]
        public void KeyNotFoundTest()
        {
            TextLocalizer.Initialize("ja", "ja", $"key,ja,en{Environment.NewLine}KeyTest,テスト,Test{Environment.NewLine}");
            try
            {
                TextLocalizer.GetText("NotExistKey");
            }
            catch (KeyNotFoundException)
            {
                // KeyNotFoundException が出れば成功
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void LastLineBrTest()
        {
            TextLocalizer.Initialize("en", "en", $"key,ja,en{Environment.NewLine}KeyTest,テスト,Test{Environment.NewLine}");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest"), "Test");

            TextLocalizer.Initialize("en", "en", $"key,ja,en{Environment.NewLine}KeyTest,テスト,Test{Environment.NewLine}");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest"), "Test");
        }

        [Test]
        public void GetDefaultTextTest()
        {
            TextLocalizer.Initialize("ja", "en", $"key,ja,en{Environment.NewLine}KeyTest,テスト{Environment.NewLine}KeyTest2,テスト2,Test2{Environment.NewLine}KeyTest3,テスト3,Test3");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest"), "テスト");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest2"), "Test2");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest3"), "Test3");
        }

        [Test]
        public void TargetLanguageStringEmptyTest()
        {
            TextLocalizer.Initialize("ja", "en", $"key,ja,en{Environment.NewLine}KeyTest,テスト{Environment.NewLine}KeyTest2,テスト2,{Environment.NewLine}KeyTest3,テスト3,Test3");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest"), "テスト");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest2"), "テスト2");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest3"), "Test3");
        }

        [Test]
        public void FailConvertTextToDic()
        {
            // Duplicated Key
            try
            {
                TextLocalizer.Initialize("ja", "ja", $"key,ja,en{Environment.NewLine}KeyTest,テスト,Test{Environment.NewLine}KeyTest,テスト,Test{Environment.NewLine}");
            }
            catch (ArgumentException e)
            {
                Debug.Log($"Test OK :{e.Message}");
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void LocalizationEnumKey()
        {
            TextLocalizer.Initialize("ja", "ja", $"key,ja,en{Environment.NewLine}KeyTest,テスト,Test{Environment.NewLine}");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest"), "テスト");
        }

        [Test]
        public void GetTextWithArgsTest()
        {
            TextLocalizer.Initialize("ja", "ja", $"key,ja{Environment.NewLine}KeyTest,テスト{{0}}{{1}}{{2}}{Environment.NewLine}");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest", "引数", "の", "Test"), "テスト引数のTest");
            TextLocalizer.Initialize("ja", "ja", $"key,ja{Environment.NewLine}KeyTest,テスト{{0}}{{1}}{{2}}{Environment.NewLine}");
            Assert.AreEqual(TextLocalizer.GetText("KeyTest", "引数", "の", "Test", "ですよ"), "テスト引数のTest");
            TextLocalizer.Initialize("ja", "ja", $"key,ja{Environment.NewLine}KeyTest,テスト{{0}}{{1}}{{2}}{Environment.NewLine}");
            try
            {
                TextLocalizer.GetText("KeyTest", "引数", "の");
            }
            catch
            {
                // catch していれば成功
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void LocalizationEnumGeneratorTest()
        {
            // Test 用のディレクトリの作成
            var testDir = Path.Combine(Application.dataPath, "IcarusTest");

            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }
            // Test 用の csv ファイルの作成
            string filePath = $"{testDir}/Test.csv";
            GenerateTestLocalizeFile(filePath);
            // Test 用の csv ファイルを読み込む
            var text = FileLoader.LoadFile(filePath);
            // その csv を GenerateEnum でコード生成
            // 生成したコードを指定先に保存
            var testFilePath = Path.Combine(testDir, "Test.cs");
            LocalizationEnumGenerator.Generate(text, testFilePath);
            // 指定先に保存したコードが一致しているかどうか Assert
            string expect =
            @"// Auto Generated File
// Menu : Tools -> LocalizationEnumGenerate
namespace Icarus.Core
{
    public enum LocalizationEnum
    {
        // カテゴリー梅
        /// <summary>
        /// ja : ありがとう
        /// en : Thx
        /// </summary>
        CommonThanks,
        // カテゴリー竹
        /// <summary>
        /// ja : 削\n除
        /// en : Del
        /// </summary>
        Delete,
        /// <summary>
        /// ja : 座標{0}
        /// en : Pos {0}
        /// </summary>
        Position,
    }
}
";

            var code = FileLoader.LoadFile(testFilePath);
            Assert.AreEqual(expect, code);

            // Test 用の csv ファイルの削除
            // Test 用に生成したコードをディレクトリごと削除
            // File.Delete(filePath);
            // File.Delete(filePath + ".meta");
            // File.Delete(testFilePath);
            // File.Delete(testFilePath + ".meta");
            // Directory.Delete(testDir);
        }

        [Test]
        public void LocalizationEnumGeneratorFailedTest()
        {
            // Test 用のディレクトリの作成
            var testDir = Path.Combine(Application.dataPath, "IcarusTest");

            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }
            // Test 用の csv ファイルの作成
            string filePath = $"{testDir}/TestFail.csv";
            GenerateTestFailedLocalizeFile(filePath);
            // Test 用の csv ファイルを読み込む
            var text = FileLoader.LoadFile(filePath);
            // その csv を GenerateEnum でコード生成
            // 生成したコードを指定先に保存
            var testFilePath = Path.Combine(testDir, "TestFail.cs");

            try
            {
                LocalizationEnumGenerator.Generate(text, testFilePath);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Duplicated Key {Delete,Sum,}", e.Message);
                return;
            }

            Assert.Fail();
        }

        private void GenerateTestLocalizeFile(string filePath)
        {
            string fileText =
@"key,ja,en
// カテゴリー梅
CommonThanks,ありがとう,Thx
// カテゴリー竹
Delete,削\n除,Del
Position,座標{0},Pos {0}"
                ;

            using (var writter = new StreamWriter(filePath))
            {
                writter.Write(fileText);
                writter.Flush();
            }
            AssetDatabase.Refresh();
        }

        private void GenerateTestFailedLocalizeFile(string filePath)
        {
            // Key被りが存在するファイル
            string fileText =
@"key,ja,en
// カテゴリー梅
CommonThanks,ありがとう,Thx
// カテゴリー竹
Delete,削\n除,Del
Delete,座標{0},Pos {0}
Sum,削\n除,Del
Sum,座標{0},Pos {0}"
                ;

            using (var writter = new StreamWriter(filePath))
            {
                writter.Write(fileText);
                writter.Flush();
            }
            AssetDatabase.Refresh();
        }
        */
    }
}