using Book_Management.Domain.Models;
using HotChocolate.Types;

namespace Book_Management.GraphQL
{
    public class AuthorType : ObjectType<Author>
    {
        protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
        {
            descriptor.Field(a => a.Id).Type<NonNullType<IdType>>();
            descriptor.Field(a => a.Name).Type<NonNullType<StringType>>();
            descriptor.Field(a => a.Email).Type<StringType>();
            descriptor.Field(a => a.Books).Type<ListType<BookType>>();
        }
    }
}
