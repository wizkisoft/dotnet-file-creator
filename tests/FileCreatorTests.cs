using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;
using Wizkisoft.DotNet.Wrapper;

namespace Wizkisoft.DotNet.IO.Test
{
    public class FileCreatorTests
    {
        public class SetDirectoryShould : IDisposable
        {
            public SetDirectoryShould()
            {
                _fixture = new Fixture()
                    .Customize(new AutoMoqCustomization());

                _directoryMock = _fixture.Freeze<Mock<IDirectory>>();
            }

            [Fact]
            public void ThrowArgumentNullException_WhenPathIsNull()
            {

                var fileCreator = _fixture.Create<FileCreator>();

                Action action = () => { fileCreator.SetDirectory(null); };

                action.Should().Throw<ArgumentNullException>(because: "A non-null path to a directory must be provided in order to set the directory");
            }

            [Fact]
            public void ThrowArgumentNullException_WhenPathIsWhitespace()
            {
                var fileCreator = _fixture.Create<FileCreator>();

                Action action = () => { fileCreator.SetDirectory(" "); };

                action.Should().Throw<ArgumentException>(because: "A path to a directory cannot be a blank string. A valid path should be provided in order to set the directory");
            }

            [Fact]
            public void DoNothing_WhenDirectoryExists()
            {
                var path = _fixture.Create<string>();
                var fileCreator = _fixture.Create<FileCreator>();

                fileCreator.SetDirectory(path);

                _directoryMock.Verify(m => m.CreateDirectory(path));
            }

            [Fact]
            public void CreateDirectory_WhenDirectoryDoesNotExist()
            {
                var path = _fixture.Create<string>();
                var fileCreator = _fixture.Create<FileCreator>();

                fileCreator.SetDirectory(path);

                _directoryMock.Verify(m => m.CreateDirectory(path));
            }

            public void Dispose()
            {
                _fixture = null;
                _directoryMock = null;
            }

            private IFixture _fixture;

            private Mock<IDirectory> _directoryMock;
        }

        public class CreateFileShould : IDisposable
        {
            public CreateFileShould()
            {
                _fixture = new Fixture()
                    .Customize(new AutoMoqCustomization());
            }

            [Fact]
            public void ConcatenatePathAndFileName_WhenPathExists()
            {
                var streamWriterMock = _fixture.Create<Mock<Func<string, IStreamWriter>>>();
                var directoryPath = _fixture.Create<string>();

                _fixture.Register<FileCreator>(() =>
                {
                    var fileCreator = new FileCreator(Mock.Of<IDirectory>(), streamWriterMock.Object);
                    fileCreator.SetDirectory(directoryPath);
                    return fileCreator;
                });

                var fileName = _fixture.Create<string>();

                var fileCreator = _fixture.Create<FileCreator>();


                fileCreator.CreateFile(fileName, _fixture.Create<List<string>>());


                var filePath = $"{directoryPath}/{fileName}";
                streamWriterMock.Verify(m => m(filePath));
            }

            [Fact]
            public void UseOnlyFileName_WhenPathDoesntExist()
            {
                var streamWriterMock = _fixture.Freeze<Mock<Func<string, IStreamWriter>>>();
                var fileCreator = new FileCreator(_fixture.Create<Mock<IDirectory>>().Object, streamWriterMock.Object);
                var fileName = _fixture.Create<string>();


                fileCreator.CreateFile(fileName, _fixture.Create<List<string>>());


                streamWriterMock.Verify(m => m(fileName));
            }

            [Fact]
            public void WriteAllLinesOfContentToFile()
            {
                var streamWriterMock = _fixture.Freeze<Mock<IStreamWriter>>();
                var content = _fixture.Create<List<string>>();
                var fileCreator = _fixture.Create<FileCreator>();


                fileCreator.CreateFile(_fixture.Create<string>(), content);


                content.ForEach(s => streamWriterMock.Verify(m => m.WriteLine(s)));
            }

            public void Dispose()
            {
                _fixture = null;
            }

            private IFixture _fixture;
        }
    }
}
