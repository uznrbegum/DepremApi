using DepremApi.Services; 
using Scalar.AspNetCore; //Scalar kütüphanesini ekliyoruz

// Web uygulamasını oluşturuyoruz& builder sayesinde ayarlarını yapıyoruz
var builder = WebApplication.CreateBuilder(args); 

//controllerları dependency injection ile ekliyoruz
//ASP.NET Core tarafından tanınabilir hale geliyo controllerlar(http request karsılar)
builder.Services.AddControllers();

//scaların kullanacağı verileri dökümanlıyor
builder.Services.AddOpenApi();

//HttpClient(AFAD API'ya request göndermek için)ı Dependency Injectiona ekliyoruz.
builder.Services.AddHttpClient<DepremService>(); // nesneyi dışardan hazır alıyor.

//Web application nesnesini oluşturuyoruz
var app = builder.Build();

if (app.Environment.IsDevelopment()) //development ortamında çalışıyorsak swagger dökümanını açıyoruz
{
    app.MapOpenApi(); //scalar endpointlere burdan ulaşabiliyor.
    app.MapScalarApiReference(); //scaların web arayüzü oluşturuluyor(GET testi için)
}

//HTTP'yi HTTPS'ye yönlendiriyoruz. (güvenlik için)
app.UseHttpsRedirection();

// controllerların endpointlerini uygulamaya bağlıyoruz
app.MapControllers();

app.Run(); //now listening on mesajı bu aşamada çıkıyor.