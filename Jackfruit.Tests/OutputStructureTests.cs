using Xunit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit.IncrementalGenerator;
using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.IncrementalGenerator.Output;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class OutputStructureTests
    {

        [Fact]
        public Task Null_codeModel_doesnt_fail()
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
            CodeFileModel codeModel = null;
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            string output = language.AddCodeFile(codeModel).Output();
            // This outputting "emptyString" is an artifact of the verivier
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }

        [Fact]
        public Task Empty_namespace_outputs_empty_file()
        {
            CodeFileModel codeModel = new("asdf");
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            string output = language.AddCodeFile(codeModel).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }

        [Fact]
        public Task Using_directives_output_correctly()
        {
            CodeFileModel codeModel = new("asdf")
            {
                Usings = { new("A"), new("B", "C") }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            string output = language.AddCodeFile(codeModel).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }

        [Fact]
        public Task Namespace_outputs_correctly()
        {
            CodeFileModel codeModel = new("asdf")
            {
                Namespace = new("A")
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            string output = language.AddCodeFile(codeModel).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }


        [Fact]
        public Task Simple_class_outputs_correctly()
        {
            ClassModel classModel = new("A")
            {
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddClass(classModel).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }

        [Fact]
        public Task Classes_output_in_namespace()
        {
            CodeFileModel codeModel = new("asdf")
            {
                Namespace = new("asdf")
                {
                    Classes = new() {
                    new("A") ,
                    new("B")}
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddCodeFile(codeModel).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }


        [Fact]
        public Task Simple_method_outputs_correctly()
        {
            MethodModel model = new("A", "string");
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }

        [Fact]
        public Task Methods_output_in_class()
        {
            ClassModel classModel = new("A")
            {
                Members = new()
                {
                    new MethodModel("B", "string"),
                    new MethodModel("C", "string")
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddClass(classModel).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }

        [Fact]
        public Task Simple_field_outputs_correctly()
        {
            FieldModel model = new("A", "string");
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddField(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }

        [Fact]
        public Task Fields_output_in_class()
        {
            ClassModel classModel = new("A")
            {
                Members = new()
                {
                    new FieldModel("A", "string"),
                    new FieldModel("B", "int")
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddClass(classModel).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }

        [Fact]
        public Task Simple_constructor_outputs_correctly()
        {
            ConstructorModel model = new("A");
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddConstructor(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }

        [Fact]
        public Task Constructor_output_in_class()
        {
            ClassModel classModel = new("A")
            {
                Members = new()
                {
                    new ConstructorModel("A"),
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddClass(classModel).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }



        [Fact]
        public Task Simple_autoProperty_outputs_correctly()
        {
            PropertyModel model = new("A", "int");
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddProperty(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }

        [Fact]
        public Task Autoproperties_output_in_class()
        {
            ClassModel classModel = new("A")
            {
                Members = new()
                {
                    new PropertyModel("A", "int"),
                    new PropertyModel("B", "int"),
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddClass(classModel).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }


        [Fact]
        public Task Simple_property_with_getter_setter_outputs_correctly()
        {
            PropertyModel model = new("A", "int")
            {
                Getter = new(),
                Setter = new()
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddProperty(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }

        [Fact]
        public Task Properties_output_together_in_class()
        {
            ClassModel classModel = new("A")
            {
                Members = new()
                {
                    new PropertyModel("A", "int")
                        {
                            Getter = new (),
                            Setter = new()
                        },
                    new PropertyModel("B", "int"),
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddClass(classModel).Output();
            return Verifier.Verify(output).UseDirectory("OutputStructuralSnaps");
        }
    }
}
