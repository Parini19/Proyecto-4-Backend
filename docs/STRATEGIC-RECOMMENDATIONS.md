# Recomendaciones Estratégicas - Cinema Management System

**Fecha:** 2025-11-03
**Objetivo:** Sistema funcional, escalable, con excelente UX/UI y mejores prácticas

---

## 🎯 Filosofía del Proyecto

### Principios Clave

1. **User First:** La experiencia del usuario es prioridad #1
2. **Developer Experience:** Código limpio, autodocumentado, fácil de mantener
3. **Scalability:** Arquitectura preparada para crecer
4. **Security:** Seguridad desde el diseño, no como agregado
5. **Performance:** Rápido desde el día 1

---

## 📐 Arquitectura Recomendada

### Backend: Clean Architecture + DDD (Domain-Driven Design)

```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTATION LAYER                      │
│                     (API Controllers)                        │
│                                                              │
│  ✅ Thin controllers (solo routing)                         │
│  ✅ DTOs para input/output                                  │
│  ✅ Validators (FluentValidation)                           │
│  ✅ Exception filters globales                              │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                     APPLICATION LAYER                        │
│                   (Use Cases / Services)                     │
│                                                              │
│  ✅ Commands & Queries (CQRS pattern)                       │
│  ✅ Business logic                                          │
│  ✅ Validation rules                                        │
│  ✅ Authorization checks                                    │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                       DOMAIN LAYER                           │
│                   (Business Models & Rules)                  │
│                                                              │
│  ✅ Entities con comportamiento                             │
│  ✅ Value Objects                                           │
│  ✅ Domain Events                                           │
│  ✅ Aggregates                                              │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                   INFRASTRUCTURE LAYER                       │
│                  (Data Access & External)                    │
│                                                              │
│  ✅ Repository implementations                              │
│  ✅ Firestore/Firebase integration                          │
│  ✅ External APIs                                           │
│  ✅ Caching (Redis)                                         │
└─────────────────────────────────────────────────────────────┘
```

### Frontend: Feature-Based Architecture

```
lib/
├── core/                          # Shared infrastructure
│   ├── theme/                     # Design system
│   │   ├── colors.dart           # Color palette
│   │   ├── typography.dart       # Text styles
│   │   ├── spacing.dart          # Consistent spacing
│   │   └── components/           # Reusable widgets
│   ├── routing/                  # GoRouter setup
│   ├── providers/                # Global Riverpod providers
│   ├── utils/                    # Helpers
│   └── services/                 # HTTP, storage, etc.
│
├── features/                      # Feature modules
│   ├── auth/
│   │   ├── data/                 # API calls, DTOs
│   │   ├── domain/               # Models, repositories
│   │   ├── presentation/         # UI, state
│   │   │   ├── providers/
│   │   │   ├── widgets/
│   │   │   └── pages/
│   │   └── auth_module.dart      # Feature exports
│   │
│   ├── movies/
│   │   ├── data/
│   │   ├── domain/
│   │   └── presentation/
│   │
│   ├── bookings/
│   └── admin/
│
└── main.dart
```

---

## 🎨 UX/UI: Design System & Patterns

### 1. Design System (Material Design 3 + Custom)

#### Color Palette

```dart
// lib/core/theme/colors.dart
class CinemaColors {
  // Brand colors
  static const primary = Color(0xFFDC2626);     // Cinema red
  static const primaryDark = Color(0xFF991B1B);
  static const primaryLight = Color(0xFFEF4444);

  // Neutrals (dark theme)
  static const background = Color(0xFF0A0A0A);
  static const surface = Color(0xFF1A1A1A);
  static const surfaceVariant = Color(0xFF2A2A2A);

  // Semantic colors
  static const success = Color(0xFF10B981);
  static const warning = Color(0xFFF59E0B);
  static const error = Color(0xFFEF4444);
  static const info = Color(0xFF3B82F6);

  // Text
  static const textPrimary = Color(0xFFFFFFFF);
  static const textSecondary = Color(0xFFA3A3A3);
  static const textTertiary = Color(0xFF737373);
}
```

#### Typography

```dart
// lib/core/theme/typography.dart
class CinemaTypography {
  static const fontFamily = 'Inter';  // Modern, readable

  // Headings
  static const h1 = TextStyle(
    fontSize: 32,
    fontWeight: FontWeight.bold,
    letterSpacing: -0.5,
    height: 1.2,
  );

  static const h2 = TextStyle(
    fontSize: 24,
    fontWeight: FontWeight.bold,
    letterSpacing: -0.3,
    height: 1.3,
  );

  // Body
  static const body1 = TextStyle(
    fontSize: 16,
    fontWeight: FontWeight.normal,
    height: 1.5,
  );

  static const body2 = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.normal,
    height: 1.5,
  );

  // Special
  static const button = TextStyle(
    fontSize: 16,
    fontWeight: FontWeight.w600,
    letterSpacing: 0.5,
  );
}
```

#### Spacing System

```dart
// lib/core/theme/spacing.dart
class CinemaSpacing {
  static const double xs = 4;
  static const double sm = 8;
  static const double md = 16;
  static const double lg = 24;
  static const double xl = 32;
  static const double xxl = 48;

  // Consistent padding
  static const pagePadding = EdgeInsets.all(md);
  static const cardPadding = EdgeInsets.all(md);
  static const sectionPadding = EdgeInsets.symmetric(vertical: lg);
}
```

### 2. Componentes Reutilizables

#### Buttons

```dart
// lib/core/theme/components/cinema_button.dart
enum ButtonVariant { primary, secondary, outline, ghost }
enum ButtonSize { small, medium, large }

class CinemaButton extends StatelessWidget {
  final String text;
  final VoidCallback? onPressed;
  final ButtonVariant variant;
  final ButtonSize size;
  final bool isLoading;
  final IconData? icon;

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: isLoading ? null : onPressed,
      style: _getButtonStyle(),
      child: isLoading
          ? SizedBox(
              height: 20,
              width: 20,
              child: CircularProgressIndicator(strokeWidth: 2),
            )
          : Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                if (icon != null) ...[
                  Icon(icon, size: _getIconSize()),
                  SizedBox(width: 8),
                ],
                Text(text),
              ],
            ),
    );
  }
}

// Uso:
CinemaButton(
  text: 'Comprar boletos',
  icon: Icons.shopping_cart,
  variant: ButtonVariant.primary,
  onPressed: () => context.go('/checkout'),
)
```

#### Cards

```dart
// lib/core/theme/components/cinema_card.dart
class CinemaCard extends StatelessWidget {
  final Widget child;
  final VoidCallback? onTap;
  final EdgeInsets? padding;
  final bool elevated;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: CinemaColors.surface,
      borderRadius: BorderRadius.circular(12),
      elevation: elevated ? 4 : 0,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: padding ?? CinemaSpacing.cardPadding,
          child: child,
        ),
      ),
    );
  }
}
```

#### Input Fields

```dart
// lib/core/theme/components/cinema_text_field.dart
class CinemaTextField extends StatelessWidget {
  final String label;
  final String? hint;
  final TextEditingController? controller;
  final String? Function(String?)? validator;
  final bool obscureText;
  final IconData? prefixIcon;
  final Widget? suffixIcon;

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: controller,
      validator: validator,
      obscureText: obscureText,
      style: CinemaTypography.body1.copyWith(
        color: CinemaColors.textPrimary,
      ),
      decoration: InputDecoration(
        labelText: label,
        hintText: hint,
        prefixIcon: prefixIcon != null ? Icon(prefixIcon) : null,
        suffixIcon: suffixIcon,
        filled: true,
        fillColor: CinemaColors.surfaceVariant,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide.none,
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(color: CinemaColors.primary, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(color: CinemaColors.error),
        ),
      ),
    );
  }
}
```

### 3. UX Patterns Recomendados

#### Loading States (Skeleton Screens)

```dart
// lib/core/theme/components/cinema_skeleton.dart
class SkeletonMovieCard extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Shimmer.fromColors(
      baseColor: CinemaColors.surface,
      highlightColor: CinemaColors.surfaceVariant,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            height: 200,
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(12),
            ),
          ),
          SizedBox(height: 8),
          Container(height: 16, width: double.infinity, color: Colors.white),
          SizedBox(height: 4),
          Container(height: 14, width: 100, color: Colors.white),
        ],
      ),
    );
  }
}
```

#### Empty States

```dart
// lib/core/theme/components/empty_state.dart
class EmptyState extends StatelessWidget {
  final IconData icon;
  final String title;
  final String description;
  final String? actionText;
  final VoidCallback? onAction;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 64, color: CinemaColors.textTertiary),
          SizedBox(height: 16),
          Text(title, style: CinemaTypography.h2),
          SizedBox(height: 8),
          Text(
            description,
            style: CinemaTypography.body2.copyWith(
              color: CinemaColors.textSecondary,
            ),
            textAlign: TextAlign.center,
          ),
          if (actionText != null) ...[
            SizedBox(height: 24),
            CinemaButton(
              text: actionText!,
              onPressed: onAction,
            ),
          ],
        ],
      ),
    );
  }
}

// Uso:
EmptyState(
  icon: Icons.movie_outlined,
  title: 'No hay películas disponibles',
  description: 'Vuelve más tarde para ver nuevos estrenos',
  actionText: 'Explorar cartelera',
  onAction: () => context.go('/movies'),
)
```

#### Error Handling

```dart
// lib/core/theme/components/error_view.dart
class ErrorView extends StatelessWidget {
  final String message;
  final VoidCallback? onRetry;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(Icons.error_outline, size: 64, color: CinemaColors.error),
          SizedBox(height: 16),
          Text('Algo salió mal', style: CinemaTypography.h2),
          SizedBox(height: 8),
          Text(
            message,
            style: CinemaTypography.body2.copyWith(
              color: CinemaColors.textSecondary,
            ),
            textAlign: TextAlign.center,
          ),
          if (onRetry != null) ...[
            SizedBox(height: 24),
            CinemaButton(
              text: 'Reintentar',
              icon: Icons.refresh,
              onPressed: onRetry,
            ),
          ],
        ],
      ),
    );
  }
}
```

#### Snackbars/Toasts Consistentes

```dart
// lib/core/utils/snackbar_helper.dart
class SnackbarHelper {
  static void showSuccess(BuildContext context, String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Row(
          children: [
            Icon(Icons.check_circle, color: CinemaColors.success),
            SizedBox(width: 12),
            Expanded(child: Text(message)),
          ],
        ),
        backgroundColor: CinemaColors.surface,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
          side: BorderSide(color: CinemaColors.success, width: 2),
        ),
      ),
    );
  }

  static void showError(BuildContext context, String message) {
    // Similar con color error
  }

  static void showInfo(BuildContext context, String message) {
    // Similar con color info
  }
}
```

---

## 🔧 Backend: Mejores Prácticas

### 1. CQRS Pattern (Command Query Responsibility Segregation)

```csharp
// Cinema.Application/Movies/Commands/CreateMovie/CreateMovieCommand.cs
public record CreateMovieCommand(
    string Title,
    string Description,
    int DurationMinutes,
    string Genre,
    string Director,
    DateTime ReleaseDate,
    decimal Rating,
    string PosterUrl,
    string TrailerUrl,
    string Classification,
    string Language
) : IRequest<CreateMovieResult>;

// Handler
public class CreateMovieCommandHandler
    : IRequestHandler<CreateMovieCommand, CreateMovieResult>
{
    private readonly IMovieRepository _repository;
    private readonly IValidator<CreateMovieCommand> _validator;

    public async Task<CreateMovieResult> Handle(
        CreateMovieCommand command,
        CancellationToken ct)
    {
        // 1. Validate
        var validationResult = await _validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // 2. Create entity
        var movie = Movie.Create(
            title: command.Title,
            description: command.Description,
            // ... otros campos
        );

        // 3. Save
        await _repository.AddAsync(movie, ct);

        // 4. Return result
        return new CreateMovieResult(movie.Id, "Movie created successfully");
    }
}
```

```csharp
// Cinema.Application/Movies/Queries/GetMovies/GetMoviesQuery.cs
public record GetMoviesQuery(
    bool OnlyActive = true,
    string? Genre = null,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedResult<MovieDto>>;

// Handler
public class GetMoviesQueryHandler
    : IRequestHandler<GetMoviesQuery, PagedResult<MovieDto>>
{
    private readonly IMovieRepository _repository;

    public async Task<PagedResult<MovieDto>> Handle(
        GetMoviesQuery query,
        CancellationToken ct)
    {
        var movies = await _repository.GetPagedAsync(
            onlyActive: query.OnlyActive,
            genre: query.Genre,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize,
            ct
        );

        return movies.ToPagedResult();
    }
}
```

### 2. FluentValidation

```csharp
// Cinema.Application/Movies/Commands/CreateMovie/CreateMovieValidator.cs
public class CreateMovieValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es requerido")
            .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida")
            .MaximumLength(2000).WithMessage("Descripción muy larga");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("La duración debe ser mayor a 0")
            .LessThan(500).WithMessage("Duración inválida");

        RuleFor(x => x.Rating)
            .InclusiveBetween(0, 5).WithMessage("Rating debe estar entre 0 y 5");

        RuleFor(x => x.PosterUrl)
            .Must(BeAValidUrl).WithMessage("URL de poster inválida")
            .When(x => !string.IsNullOrEmpty(x.PosterUrl));

        RuleFor(x => x.ReleaseDate)
            .LessThanOrEqualTo(DateTime.Now.AddYears(2))
            .WithMessage("Fecha de estreno muy lejana");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
```

### 3. Result Pattern (en lugar de excepciones)

```csharp
// Cinema.Domain/Common/Result.cs
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}

// Uso:
public async Task<Result<Movie>> CreateMovieAsync(CreateMovieCommand command)
{
    // Validations
    if (string.IsNullOrEmpty(command.Title))
        return Result<Movie>.Failure("Title is required");

    // Business logic
    var movie = Movie.Create(command.Title, ...);

    await _repository.AddAsync(movie);

    return Result<Movie>.Success(movie);
}
```

### 4. Domain Events

```csharp
// Cinema.Domain/Events/MovieCreatedEvent.cs
public record MovieCreatedEvent(
    string MovieId,
    string Title,
    DateTime CreatedAt
) : IDomainEvent;

// Cinema.Application/Movies/EventHandlers/MovieCreatedEventHandler.cs
public class MovieCreatedEventHandler : INotificationHandler<MovieCreatedEvent>
{
    private readonly IEmailService _emailService;

    public async Task Handle(MovieCreatedEvent notification, CancellationToken ct)
    {
        // Enviar notificación a admins
        await _emailService.SendMovieCreatedNotificationAsync(
            notification.MovieId,
            notification.Title,
            ct
        );
    }
}
```

### 5. Repository Pattern Mejorado

```csharp
// Cinema.Application/Common/IRepository.cs
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default
    );
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task<bool> ExistsAsync(string id, CancellationToken ct = default);
}

// Cinema.Application/Movies/IMovieRepository.cs
public interface IMovieRepository : IRepository<Movie>
{
    // Queries específicas de Movie
    Task<IReadOnlyList<Movie>> GetActiveMoviesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Movie>> GetMoviesByGenreAsync(
        string genre,
        CancellationToken ct = default
    );
    Task<IReadOnlyList<Movie>> SearchMoviesAsync(
        string searchTerm,
        CancellationToken ct = default
    );
}
```

### 6. DTOs & Mapping (AutoMapper)

```csharp
// Cinema.Application/Movies/DTOs/MovieDto.cs
public record MovieDto(
    string Id,
    string Title,
    string Description,
    int DurationMinutes,
    string Genre,
    string Director,
    DateTime ReleaseDate,
    decimal Rating,
    string PosterUrl,
    string TrailerUrl,
    string Classification,
    bool IsActive
);

// Cinema.Application/Movies/Mappings/MovieMappingProfile.cs
public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        CreateMap<Movie, MovieDto>();
        CreateMap<CreateMovieCommand, Movie>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
    }
}
```

### 7. Exception Handling Global

```csharp
// Cinema.Api/Middleware/GlobalExceptionMiddleware.cs
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error");
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access");
            await HandleUnauthorizedExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException exception)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = "Validation failed",
            errors = exception.Errors.Select(e => new
            {
                field = e.PropertyName,
                message = e.ErrorMessage
            })
        };

        return context.Response.WriteAsJsonAsync(response);
    }

    // ... otros handlers
}
```

---

## 🎨 Frontend: Mejores Prácticas

### 1. State Management con Riverpod (Recomendado)

```dart
// lib/features/movies/presentation/providers/movies_provider.dart
@riverpod
class MoviesNotifier extends _$MoviesNotifier {
  @override
  FutureOr<List<Movie>> build() async {
    return await _fetchMovies();
  }

  Future<List<Movie>> _fetchMovies() async {
    final movieService = ref.read(movieServiceProvider);
    return await movieService.getActiveMovies();
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() => _fetchMovies());
  }

  Future<void> addMovie(CreateMovieDto dto) async {
    final movieService = ref.read(movieServiceProvider);
    await movieService.createMovie(dto);
    await refresh();
  }
}

// Uso en UI:
class MoviesPage extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final moviesAsync = ref.watch(moviesNotifierProvider);

    return moviesAsync.when(
      loading: () => SkeletonMovieList(),
      error: (error, stack) => ErrorView(
        message: error.toString(),
        onRetry: () => ref.refresh(moviesNotifierProvider),
      ),
      data: (movies) {
        if (movies.isEmpty) {
          return EmptyState(
            icon: Icons.movie_outlined,
            title: 'No hay películas',
            description: 'Agrega tu primera película',
          );
        }

        return ListView.builder(
          itemCount: movies.length,
          itemBuilder: (context, index) => MovieCard(movie: movies[index]),
        );
      },
    );
  }
}
```

### 2. GoRouter para Navegación

```dart
// lib/core/routing/app_router.dart
final goRouterProvider = Provider<GoRouter>((ref) {
  final authState = ref.watch(authStateProvider);

  return GoRouter(
    initialLocation: '/',
    redirect: (context, state) {
      final isLoggedIn = authState.value?.isLoggedIn ?? false;
      final isAdmin = authState.value?.isAdmin ?? false;
      final isGoingToLogin = state.matchedLocation == '/login';

      // Redirect no autenticados a login
      if (!isLoggedIn && !isGoingToLogin) {
        return '/login';
      }

      // Redirect admin routes si no es admin
      if (state.matchedLocation.startsWith('/admin') && !isAdmin) {
        return '/';
      }

      return null;
    },
    routes: [
      GoRoute(
        path: '/',
        builder: (context, state) => HomePage(),
      ),
      GoRoute(
        path: '/login',
        builder: (context, state) => LoginPage(),
      ),
      GoRoute(
        path: '/movies',
        builder: (context, state) => MoviesPage(),
        routes: [
          GoRoute(
            path: ':id',
            builder: (context, state) {
              final id = state.pathParameters['id']!;
              return MovieDetailPage(movieId: id);
            },
          ),
        ],
      ),
      GoRoute(
        path: '/admin',
        redirect: (context, state) {
          final isAdmin = authState.value?.isAdmin ?? false;
          return isAdmin ? null : '/';
        },
        routes: [
          GoRoute(
            path: 'movies',
            builder: (context, state) => AdminMoviesPage(),
          ),
          // ... más rutas admin
        ],
      ),
    ],
  );
});
```

### 3. Service Layer Pattern

```dart
// lib/features/movies/data/movie_service.dart
@riverpod
MovieService movieService(MovieServiceRef ref) {
  final apiClient = ref.watch(apiClientProvider);
  return MovieService(apiClient);
}

class MovieService {
  final ApiClient _apiClient;

  MovieService(this._apiClient);

  Future<List<Movie>> getActiveMovies() async {
    try {
      final response = await _apiClient.get('/api/movies');
      return (response.data as List)
          .map((json) => Movie.fromJson(json))
          .toList();
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<Movie> getMovieById(String id) async {
    try {
      final response = await _apiClient.get('/api/movies/$id');
      return Movie.fromJson(response.data);
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<Movie> createMovie(CreateMovieDto dto) async {
    try {
      final response = await _apiClient.post(
        '/api/movies',
        data: dto.toJson(),
      );
      return Movie.fromJson(response.data);
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  AppException _handleError(DioException e) {
    if (e.response != null) {
      switch (e.response!.statusCode) {
        case 400:
          return ValidationException(e.response!.data['message']);
        case 401:
          return UnauthorizedException();
        case 404:
          return NotFoundException(e.response!.data['message']);
        default:
          return ServerException(e.response!.data['message']);
      }
    }
    return NetworkException('No internet connection');
  }
}
```

### 4. Secure Storage para Tokens

```dart
// lib/core/services/secure_storage_service.dart
@riverpod
SecureStorageService secureStorageService(SecureStorageServiceRef ref) {
  return SecureStorageService();
}

class SecureStorageService {
  final _storage = const FlutterSecureStorage();

  Future<void> saveToken(String token) async {
    await _storage.write(key: 'auth_token', value: token);
  }

  Future<String?> getToken() async {
    return await _storage.read(key: 'auth_token');
  }

  Future<void> deleteToken() async {
    await _storage.delete(key: 'auth_token');
  }

  Future<void> saveUserData(User user) async {
    await _storage.write(key: 'user_data', value: jsonEncode(user.toJson()));
  }

  Future<User?> getUserData() async {
    final data = await _storage.read(key: 'user_data');
    if (data == null) return null;
    return User.fromJson(jsonDecode(data));
  }
}
```

### 5. Dio Interceptor para Auth

```dart
// lib/core/services/api_client.dart
@riverpod
Dio dio(DioRef ref) {
  final dio = Dio(BaseOptions(
    baseUrl: AppConfig.apiBaseUrl,
    connectTimeout: const Duration(seconds: 5),
    receiveTimeout: const Duration(seconds: 10),
  ));

  // Auth interceptor
  dio.interceptors.add(InterceptorsWrapper(
    onRequest: (options, handler) async {
      final storage = ref.read(secureStorageServiceProvider);
      final token = await storage.getToken();

      if (token != null) {
        options.headers['Authorization'] = 'Bearer $token';
      }

      return handler.next(options);
    },
    onError: (error, handler) async {
      if (error.response?.statusCode == 401) {
        // Token expirado, logout
        final authNotifier = ref.read(authNotifierProvider.notifier);
        await authNotifier.logout();
        // Redirect a login
      }
      return handler.next(error);
    },
  ));

  // Logging interceptor (solo development)
  if (kDebugMode) {
    dio.interceptors.add(LogInterceptor(
      requestBody: true,
      responseBody: true,
    ));
  }

  return dio;
}
```

---

## 📦 Estructura de Proyecto Completa

### Backend

```
Cinema/
├── src/
│   ├── Cinema.Api/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   │   ├── GlobalExceptionMiddleware.cs
│   │   │   └── UserActionAuditMiddleware.cs
│   │   ├── Filters/
│   │   │   └── ValidationFilter.cs
│   │   └── Program.cs
│   │
│   ├── Cinema.Application/
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   ├── Behaviours/        # MediatR pipelines
│   │   │   └── Exceptions/
│   │   ├── Movies/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateMovie/
│   │   │   │   ├── UpdateMovie/
│   │   │   │   └── DeleteMovie/
│   │   │   ├── Queries/
│   │   │   │   ├── GetMovies/
│   │   │   │   └── GetMovieById/
│   │   │   ├── DTOs/
│   │   │   └── Validators/
│   │   └── ... (otras features)
│   │
│   ├── Cinema.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   ├── Enums/
│   │   └── Exceptions/
│   │
│   └── Cinema.Infrastructure/
│       ├── Repositories/
│       ├── Services/
│       ├── Caching/
│       └── DependencyInjection.cs
│
├── tests/
│   ├── Cinema.Api.Tests/
│   ├── Cinema.Application.Tests/
│   └── Cinema.Domain.Tests/
│
└── docs/
```

### Frontend

```
cinema_frontend/
├── lib/
│   ├── core/
│   │   ├── theme/
│   │   │   ├── colors.dart
│   │   │   ├── typography.dart
│   │   │   ├── spacing.dart
│   │   │   ├── theme.dart
│   │   │   └── components/
│   │   │       ├── cinema_button.dart
│   │   │       ├── cinema_card.dart
│   │   │       ├── cinema_text_field.dart
│   │   │       ├── skeleton_loader.dart
│   │   │       ├── empty_state.dart
│   │   │       └── error_view.dart
│   │   ├── routing/
│   │   │   └── app_router.dart
│   │   ├── services/
│   │   │   ├── api_client.dart
│   │   │   └── secure_storage_service.dart
│   │   ├── utils/
│   │   │   ├── validators.dart
│   │   │   ├── formatters.dart
│   │   │   └── constants.dart
│   │   └── providers/
│   │       └── global_providers.dart
│   │
│   ├── features/
│   │   ├── auth/
│   │   │   ├── data/
│   │   │   │   ├── auth_service.dart
│   │   │   │   └── dtos/
│   │   │   ├── domain/
│   │   │   │   ├── user.dart
│   │   │   │   └── auth_repository.dart
│   │   │   └── presentation/
│   │   │       ├── providers/
│   │   │       │   └── auth_provider.dart
│   │   │       ├── widgets/
│   │   │       │   ├── login_form.dart
│   │   │       │   └── social_login_buttons.dart
│   │   │       └── pages/
│   │   │           └── login_page.dart
│   │   │
│   │   ├── movies/
│   │   │   ├── data/
│   │   │   │   ├── movie_service.dart
│   │   │   │   └── dtos/
│   │   │   ├── domain/
│   │   │   │   └── movie.dart
│   │   │   └── presentation/
│   │   │       ├── providers/
│   │   │       ├── widgets/
│   │   │       │   ├── movie_card.dart
│   │   │       │   ├── movie_grid.dart
│   │   │       │   └── movie_filter.dart
│   │   │       └── pages/
│   │   │           ├── movies_page.dart
│   │   │           └── movie_detail_page.dart
│   │   │
│   │   └── ... (bookings, admin, etc.)
│   │
│   └── main.dart
│
├── test/
│   ├── widget_test/
│   ├── integration_test/
│   └── unit_test/
│
└── pubspec.yaml
```

---

## 🚀 Plan de Implementación Recomendado

### FASE 0: Setup & Fundación (1 semana)

**Prioridad:** 🔴 CRÍTICA

#### Backend
1. ✅ Configurar JWT Key y Firebase credentials
2. ✅ Implementar BCrypt para passwords
3. ✅ Setup MediatR + FluentValidation
4. ✅ Global Exception Middleware
5. ✅ Configurar AutoMapper
6. ✅ Setup testing framework

#### Frontend
1. ✅ Migrar a GoRouter
2. ✅ Migrar a Riverpod completo
3. ✅ Crear Design System (colors, typography, spacing)
4. ✅ Componentes base (button, card, textfield)
5. ✅ Implementar secure storage
6. ✅ Dio con interceptor de auth

**Entregable:** Fundación sólida con mejores prácticas

---

### FASE 1: Movies Feature Completo (1 semana)

**Prioridad:** 🔴 ALTA

#### Backend
1. ✅ Entidad Movie completa (con campos adicionales)
2. ✅ Commands: CreateMovie, UpdateMovie, DeleteMovie
3. ✅ Queries: GetMovies (paginado), GetMovieById, SearchMovies
4. ✅ Validators para cada command
5. ✅ FirestoreMovieRepository completo
6. ✅ DTOs y mappings
7. ✅ Unit tests

#### Frontend
1. ✅ Movie domain model
2. ✅ MovieService con todos los métodos
3. ✅ MoviesProvider con Riverpod
4. ✅ MoviesPage (customer) - grid con filtros
5. ✅ MovieDetailPage - con trailer, rating, horarios
6. ✅ AdminMoviesPage - CRUD completo
7. ✅ Movie widgets (MovieCard, MovieGrid, MovieFilter)

**Entregable:** Feature completa de Movies con UX excelente

---

### FASE 2: Bookings & Screenings (1.5 semanas)

**Prioridad:** 🔴 ALTA

#### Backend
1. ✅ Entidades: Screening, TheaterRoom, Booking, Seat
2. ✅ Commands & Queries para cada entidad
3. ✅ Lógica de negocio:
   - Validar solapamiento de horarios
   - Reservas atómicas (transactions)
   - Timeout de reservas (15 min)
4. ✅ Repositories completos
5. ✅ Integration tests

#### Frontend
1. ✅ Screening selection (por película)
2. ✅ Seat selection interactivo
3. ✅ Booking flow completo
4. ✅ Timer de reserva (countdown)
5. ✅ My Bookings page
6. ✅ Admin Screenings management

**Entregable:** Sistema de reservas funcional

---

### FASE 3: Food Orders & Payment Mock (1 semana)

**Prioridad:** 🟡 MEDIA

#### Backend
1. ✅ FoodCombo & FoodOrder CRUD
2. ✅ Calcular precios automáticamente
3. ✅ Mock Payment Service
4. ✅ Order status management

#### Frontend
1. ✅ Food menu con carrito
2. ✅ Checkout flow
3. ✅ Payment mock UI
4. ✅ Order confirmation

**Entregable:** Sistema de alimentos y pago simulado

---

### FASE 4: Admin Dashboard & Analytics (1 semana)

**Prioridad:** 🟡 MEDIA

#### Backend
1. ✅ Reports endpoints (ventas, ocupación)
2. ✅ Aggregation queries
3. ✅ Export to PDF/Excel

#### Frontend
1. ✅ Admin dashboard home con métricas
2. ✅ Charts (fl_chart)
3. ✅ Reports page
4. ✅ Admin CRUD pages completas

**Entregable:** Dashboard administrativo completo

---

### FASE 5: Polish & Testing (1 semana)

**Prioridad:** 🟢 IMPORTANTE

1. ✅ Unit tests (coverage > 80%)
2. ✅ Integration tests
3. ✅ E2E tests
4. ✅ Performance optimization
5. ✅ Accessibility
6. ✅ Dark/Light theme toggle
7. ✅ Animations & transitions

**Entregable:** Aplicación pulida y testeada

---

## 📊 Métricas de Éxito

### UX/UI
- ✅ Time to First Interaction < 2s
- ✅ Todas las acciones tienen feedback visual
- ✅ Loading states en todas las operaciones async
- ✅ Error messages claros y accionables
- ✅ Mobile-first, responsive design
- ✅ Accessibility score > 90 (Lighthouse)

### Backend
- ✅ API response time < 200ms (p95)
- ✅ Test coverage > 80%
- ✅ Zero critical security vulnerabilities
- ✅ Código autodocumentado (clean code)
- ✅ Logs estructurados con correlation IDs

### Frontend
- ✅ App bundle size < 2MB (web)
- ✅ First Contentful Paint < 1.5s
- ✅ No frame drops (60 fps)
- ✅ Offline-first capabilities (future)

---

## 🎯 Decisión Final: ¿Qué Hacer?

### Mi Recomendación: **ENFOQUE INCREMENTAL**

1. **Semana 1: FUNDACIÓN**
   - Setup completo de mejores prácticas
   - Design System
   - CQRS + FluentValidation
   - Riverpod + GoRouter

2. **Semana 2-3: MVP CORE**
   - Movies feature completo
   - Bookings básico
   - UX pulido desde el inicio

3. **Semana 4-5: FEATURES AVANZADAS**
   - Admin dashboard
   - Analytics
   - Food orders

4. **Semana 6: POLISH**
   - Testing
   - Performance
   - Launch prep

### ✅ Ventajas de este enfoque:

- **UX excelente desde el día 1** (no refactor después)
- **Código escalable** (patrones correctos desde inicio)
- **Velocidad de desarrollo** (componentes reutilizables)
- **Fácil mantenimiento** (arquitectura limpia)
- **Onboarding rápido** (código autodocumentado)

---

## 📝 Próximo Paso INMEDIATO

**Te recomiendo empezar con:**

1. **Configurar credenciales** (30 min)
   - JWT Key
   - Firebase Service Account

2. **Implementar BCrypt** (30 min)
   - Seguridad primero

3. **Crear Design System básico** (2 horas)
   - Colors, Typography, Spacing
   - 3 componentes base (Button, Card, TextField)

4. **Setup Riverpod + GoRouter** (2 horas)
   - Migración de navegación
   - Provider structure

5. **Implementar Movies CRUD backend** (4 horas)
   - Con CQRS
   - Con FluentValidation
   - Con Result pattern

**Total: 1 día de trabajo para sentar bases sólidas**

---

## ❓ ¿Quieres que empecemos?

Puedo ayudarte a:
1. ✅ Implementar el Design System completo
2. ✅ Setup de CQRS + MediatR en backend
3. ✅ Migrar a Riverpod + GoRouter
4. ✅ Crear componentes reutilizables
5. ✅ Implementar Movies feature completo con mejores prácticas

**¿Por cuál empezamos?** 🚀
