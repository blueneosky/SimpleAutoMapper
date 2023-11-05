
# SimpleAutoMapper
*! early version !*

This auto mapper is intended to be config less as possible.

```csharp
var mapper = new SimpleAutoMapper();
// ...
var dto = mapper.Map<UserDao, UserDto>(dao);

class UserDao {
  public string UserName { get; set; }
  public int Age { get; set; }
}

class UserDto {
  public string UserName { get; set; }
  public int Age { get; set; }
}
```

## Current features

- Mapping on demande
- Thread safe
- Fields and properties mapping (value types and string)

## Mapping commands available
```csharp
UserDao dao = new USerDao() { /* ... */ };
IEnumerable<UserDao> daos = new List<UserDao>();

// recommanded
UserDto dto = mapper.Map<UserDao, UserDto>(dao);
ICollection<UserDto> dtos = mapper.MapCollection<UserDao, UserDto>(daos);

// fluent notation (static type mapping) - less efficient
UserDto dto = mapper.MapFrom(dao).To<UserDto>();
ICollection<UserDto> dtos = mapper.MapCollectionFrom(daos).To<UserDto>();

// more user friendly (dynamic type mapping) - less efficient
UserDto dto = mapper.MapTo<UserDto>(dao);
ICollection<UserDto> dtos = mapper.MapCollectionTo<UserDto>(daos);
```

## Features to come
*The file Todo.txt contains all wanted features*

Main features are :
- Full mapping (deep mapping, standard object like collections, enum convertion)
- Configuration :
-- explicit type mapping
-- explicit member mapping
-- setup members to ignore
-- cycle management
- DI
- ...