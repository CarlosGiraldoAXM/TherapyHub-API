# Cómo generar hashes BCrypt para contraseñas

## Opción 1: Usando C# (Console App o endpoint temporal)

```csharp
using BCrypt.Net;

string password = "Admin123";
string hash = BCrypt.Net.BCrypt.HashPassword(password);
Console.WriteLine($"Hash: {hash}");
```

## Opción 2: Usando PowerShell

```powershell
# Instalar el módulo si no lo tienes
# Install-Module -Name BCrypt.Net

# O usar una herramienta online como: https://bcrypt-generator.com/
```

## Opción 3: Crear un endpoint temporal en la API

Puedes crear un endpoint temporal para generar hashes:

```csharp
[HttpPost("generate-hash")]
public IActionResult GenerateHash([FromBody] string password)
{
    var hash = BCrypt.Net.BCrypt.HashPassword(password);
    return Ok(new { hash });
}
```

## Valores de TipoUsuario

- **1** = Administrador
- **2** = Terapeuta  
- **3** = Paciente

## Ejemplo de hash generado

Para la contraseña "Admin123", un hash típico sería:
```
$2a$11$KIXQZQZQZQZQZQZQZQZQZ.QZQZQZQZQZQZQZQZQZQZQZQZQZQZQZ
```

**IMPORTANTE**: Cada vez que generes un hash, será diferente aunque uses la misma contraseña. Esto es normal y esperado con BCrypt.
