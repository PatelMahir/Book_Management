using Book_Management.Domain.Models;
using HotChocolate.Types;

namespace Book_Management.GraphQL
{
    public class BookType : ObjectType<Book>
    {
        protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
        {
            descriptor.Field(b => b.Id).Type<NonNullType<IdType>>();
            descriptor.Field(b => b.Title).Type<NonNullType<StringType>>();
            descriptor.Field(b => b.ISBN).Type<NonNullType<StringType>>();
            descriptor.Field(b => b.PublicationYear).Type<NonNullType<IntType>>();
            descriptor.Field(b => b.Author).Type<AuthorType>();
        }
    }
}
