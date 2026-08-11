"use client";

import { useEffect, useState } from "react";

type Deprem = {
  location: string;
  magnitude: number;
  depth: number;
  date: string;
};

export default function Home() {
  const [depremler, setDepremler] = useState<Deprem[]>([]);
  const [loading, setLoading] = useState(false);

  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [location, setLocation] = useState("");
  const [minMagnitude, setMinMagnitude] = useState("");
  const [maxMagnitude, setMaxMagnitude] = useState("");

  const [lastUpdated, setLastUpdated] = useState<Date | null>(null);
  const [view, setView] = useState<"table" | "map">("table");

  async function depremleriGetir() {
    setLoading(true);

    try {
      const params = new URLSearchParams();

      if (startDate) {
        params.append("startDate", startDate);
      }

      if (endDate) {
        params.append("endDate", endDate);
      }

      if (location) {
        params.append("location", location);
      }

      if (minMagnitude) {
        params.append("minMagnitude", minMagnitude);
      }

      if (maxMagnitude) {
        params.append("maxMagnitude", maxMagnitude);
      }

      const url = `http://localhost:5173/api/Deprem?${params.toString()}`;

      console.log("API URL:", url);

      const response = await fetch(url);

      if (!response.ok) {
        throw new Error("API isteği başarısız.");
      }

      const data = await response.json();

      console.log("API'den gelen veri:", data);

      setDepremler(data);
      setLastUpdated(new Date());
    } catch (error) {
      console.error("Hata:", error);
      setDepremler([]);
    } finally {
      setLoading(false);
    }
  }

  // Sayfa açıldığında güncel depremleri getir
  useEffect(() => {
    depremleriGetir();
  }, []);

  function formatDate(date: string) {
    return new Date(date).toLocaleString("tr-TR", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  }

  function getMagnitudeClass(magnitude: number) {
    if (magnitude >= 5) return "magnitude danger";
    if (magnitude >= 4) return "magnitude warning";
    if (magnitude >= 3) return "magnitude medium";
    return "magnitude low";
  }

  const strongestEarthquake =
    depremler.length > 0
      ? Math.max(...depremler.map((deprem) => deprem.magnitude))
      : 0;

  const averageDepth =
    depremler.length > 0
      ? (
          depremler.reduce((sum, deprem) => sum + deprem.depth, 0) /
          depremler.length
        ).toFixed(1)
      : "0";

  return (
    <div className="app-shell">

      {/* SIDEBAR */}
      <aside className="sidebar">

        <div className="brand">

          <div className="brand-icon">
            <span>〰</span>
          </div>

          <div className="brand-text">
            <h2>DEPREM</h2>
            <span>TAKİP</span>
          </div>

        </div>

        <div className="sidebar-section">

          <p className="sidebar-label">MENÜ</p>

          <button className="nav-item active">
            <span>⌂</span>
            Ana Sayfa
          </button>

          <button
            className="nav-item"
            onClick={() =>
              document
                .getElementById("search-section")
                ?.scrollIntoView({ behavior: "smooth" })
            }
          >
            <span>⌕</span>
            Arama ve Filtreleme
          </button>

          <button
            className="nav-item"
            onClick={() =>
              document
                .getElementById("earthquakes-section")
                ?.scrollIntoView({ behavior: "smooth" })
            }
          >
            <span>◉</span>
            Depremler
          </button>

          <button
            className="nav-item"
            onClick={() =>
              document
                .getElementById("analytics-section")
                ?.scrollIntoView({ behavior: "smooth" })
            }
          >
            <span>◫</span>
            Analizler
          </button>

        </div>

      </aside>


      {/* MAIN CONTENT */}
      <main className="main-content">

        {/* HEADER */}
        <header className="topbar">

          <div>

            <p className="eyebrow">
              DEPREM TAKİP
            </p>

            <h1>
              Güncel Deprem Hareketleri
            </h1>

            <p className="subtitle">
              Türkiye ve çevresindeki güncel deprem hareketlerini takip edin.
            </p>

          </div>


          <div className="live-status">

            <span className="live-dot"></span>

            <div>

              <strong>GÜNCEL</strong>

              <span>
                {lastUpdated
                  ? `Son güncelleme ${lastUpdated.toLocaleTimeString(
                      "tr-TR",
                      {
                        hour: "2-digit",
                        minute: "2-digit",
                      }
                    )}`
                  : "Güncelleniyor..."}
              </span>

            </div>

          </div>

        </header>


        {/* STATISTICS */}
        <section className="stats-grid">

          <div className="stat-card">

            <div className="stat-top">
              <span>Toplam Deprem</span>
              <span className="stat-icon">◌</span>
            </div>

            <strong>{depremler.length}</strong>

            <p>Mevcut deprem kaydı</p>

          </div>


          <div className="stat-card">

            <div className="stat-top">
              <span>En Büyük Deprem</span>
              <span className="stat-icon">↗</span>
            </div>

            <strong>
              {strongestEarthquake > 0
                ? strongestEarthquake.toFixed(1)
                : "—"}
            </strong>

            <p>Büyüklük değeri</p>

          </div>


          <div className="stat-card">

            <div className="stat-top">
              <span>Ortalama Derinlik</span>
              <span className="stat-icon">↓</span>
            </div>

            <strong>
              {averageDepth} km
            </strong>

            <p>Ortalama deprem derinliği</p>

          </div>


          <div className="stat-card">

            <div className="stat-top">
              <span>Son Güncelleme</span>
              <span className="stat-icon">◷</span>
            </div>

            <strong>
              {lastUpdated
                ? lastUpdated.toLocaleTimeString("tr-TR", {
                    hour: "2-digit",
                    minute: "2-digit",
                  })
                : "—"}
            </strong>

            <p>Veri yenileme zamanı</p>

          </div>

        </section>


        {/* SEARCH & FILTER */}
        <section
          className="panel"
          id="search-section"
        >

          <div className="section-heading">

            <div>

              <p className="eyebrow">
                VERİ SORGULAMA
              </p>

              <h2>
                Arama ve Filtreleme
              </h2>

            </div>

            <span className="section-description">
              Deprem kayıtlarını tarih, konum ve büyüklüğe göre filtreleyin.
            </span>

          </div>


          <div className="filter-grid">

            <label>

              <span>
                Başlangıç Tarihi
              </span>

              <input
                type="date"
                value={startDate}
                onChange={(e) =>
                  setStartDate(e.target.value)
                }
              />

            </label>


            <label>

              <span>
                Bitiş Tarihi
              </span>

              <input
                type="date"
                value={endDate}
                onChange={(e) =>
                  setEndDate(e.target.value)
                }
              />

            </label>


            <label className="location-input">

              <span>
                Konum
              </span>

              <input
                type="text"
                placeholder="Örn. İstanbul"
                value={location}
                onChange={(e) =>
                  setLocation(e.target.value)
                }
              />

            </label>


            <label>

              <span>
                Minimum Büyüklük
              </span>

              <input
                type="number"
                step="0.1"
                placeholder="0.0"
                value={minMagnitude}
                onChange={(e) =>
                  setMinMagnitude(e.target.value)
                }
              />

            </label>


            <label>

              <span>
                Maksimum Büyüklük
              </span>

              <input
                type="number"
                step="0.1"
                placeholder="7.0"
                value={maxMagnitude}
                onChange={(e) =>
                  setMaxMagnitude(e.target.value)
                }
              />

            </label>


            <button
              className="search-button"
              onClick={depremleriGetir}
              disabled={loading}
            >

              {loading
                ? "Aranıyor..."
                : "Depremleri Ara"}

              <span>→</span>

            </button>

          </div>

        </section>


        {/* EARTHQUAKES */}
        <section
          className="panel"
          id="earthquakes-section"
        >

          <div className="section-heading">

            <div>

              <p className="eyebrow">
                DEPREM VERİLERİ
              </p>

              <h2>
                Son Depremler
              </h2>

            </div>


            <div className="view-switch">

              <button
                className={
                  view === "table"
                    ? "view-active"
                    : ""
                }
                onClick={() =>
                  setView("table")
                }
              >
                Tablo
              </button>


              <button
                className={
                  view === "map"
                    ? "view-active"
                    : ""
                }
                onClick={() =>
                  setView("map")
                }
              >
                Harita
              </button>

            </div>

          </div>


          {loading ? (

            <div className="empty-state">

              <div className="loader"></div>

              <p>
                Deprem verileri yükleniyor...
              </p>

            </div>

          ) : depremler.length === 0 ? (

            <div className="empty-state">

              <div className="empty-icon">
                〰
              </div>

              <h3>
                Deprem verisi bulunamadı
              </h3>

              <p>
                Seçtiğiniz filtrelere uygun herhangi
                bir deprem kaydı bulunamadı.
              </p>

            </div>

          ) : view === "table" ? (

            <div className="table-wrapper">

              <table>

                <thead>

                  <tr>

                    <th>
                      Tarih ve Saat
                    </th>

                    <th>
                      Konum
                    </th>

                    <th>
                      Büyüklük
                    </th>

                    <th>
                      Derinlik
                    </th>

                  </tr>

                </thead>


                <tbody>

                  {depremler.map(
                    (deprem, index) => (

                      <tr
                        key={`${deprem.date}-${index}`}
                      >

                        <td className="date-cell">
                          {formatDate(
                            deprem.date
                          )}
                        </td>


                        <td className="location-cell">
                          {deprem.location}
                        </td>


                        <td>

                          <span
                            className={getMagnitudeClass(
                              deprem.magnitude
                            )}
                          >
                            {deprem.magnitude.toFixed(1)}
                          </span>

                        </td>


                        <td className="depth-cell">
                          {deprem.depth.toFixed(2)} km
                        </td>

                      </tr>

                    )
                  )}

                </tbody>

              </table>

            </div>

          ) : (

            <div className="map-placeholder">

              <div className="map-content">

                <div className="map-grid"></div>

                <div className="map-center">

                  <span className="map-pulse"></span>
                  <span className="map-pulse"></span>
                  <span className="map-pulse"></span>

                </div>


                <div className="map-message">

                  <strong>
                    Deprem Haritası
                  </strong>

                  <span>
                    Etkileşimli harita yakında
                    kullanıma sunulacaktır.
                  </span>

                </div>

              </div>

            </div>

          )}

        </section>


        {/* ANALYTICS */}
        <section
          className="panel"
          id="analytics-section"
        >

          <div className="section-heading">

            <div>

              <p className="eyebrow">
                VERİ ANALİZİ
              </p>

              <h2>
                Analizler
              </h2>

            </div>

            <span className="section-description">
              Deprem verilerine ait istatistikler
              ve analizler burada gösterilecektir.
            </span>

          </div>


          <div className="empty-state">

            <div className="empty-icon">
              ◫
            </div>

            <h3>
              Analizler hazırlanıyor
            </h3>

            <p>
              Deprem verileri üzerinden
              istatistiksel analizler yakında
              burada görüntülenecektir.
            </p>

          </div>

        </section>


        {/* FOOTER */}
        <footer>

          <div>

            <strong>
              DEPREM TAKİP
            </strong>

            <span>
              Güncel deprem verilerini
              takip etmenize yardımcı olur.
            </span>

          </div>


          <div className="footer-right">

            <span>
              Veri kaynağı: AFAD
            </span>

            <span className="footer-status">

              <span className="status-dot"></span>

              Güncel veri

            </span>

          </div>

        </footer>

      </main>

    </div>
  );
}