## Informasi Program
- **Nama Program**: Ornamen Papua in Godot
- **Penulis**: Ahmad Fatan Haidar
- **NIM**: 231520434
- **Kelas**: D4-2B

## Deskripsi Program
Program ini merupakan visualisasi interaktif berbasis Godot Engine yang menampilkan tiga objek kearifan lokal dari Papua:

1. **Rumah Honai** - Rumah tradisional suku Dani di Papua yang memiliki bentuk bulat dengan atap kerucut yang khas
2. **Tas Noken** - Tas tradisional Papua yang dianyam dengan teknik khusus dan memiliki motif-motif khas
3. **Sate Ulat Sagu** - Kuliner khas Papua berupa ulat sagu yang dibakar dengan tusuk sate

Visualisasi dibuat secara programatik menggunakan algoritma grafika komputer dasar seperti:
- Algoritma Bresenham untuk menggambar garis
- Algoritma DDA untuk plotting garis
- Algoritma Midpoint untuk membuat lingkaran dan elips
- Teknik transformasi (translasi, rotasi, dan scaling)
- Teknik animasi dan interpolasi untuk pergerakan objek

Program menyajikan tampilan dalam beberapa mode:
- Mode kerangka (outline)
- Mode berwarna (dengan pengisian warna)
- Mode animasi dengan transformasi objek

## Jadwal Planning Progress

### Tahap 1: Persiapan dan Desain (16 Maret - 22 Maret)
- Penentuan objek kearifan lokal Papua yang akan divisualisasikan
- Studi literatur tentang algoritma grafika komputer dasar
- Persiapan lingkungan pengembangan Godot Engine
- Pembuatan sketsa awal objek-objek yang akan digambar

### Tahap 2: Implementasi Primitif Dasar (23 Maret - 29 Maret)
- Implementasi algoritma Bresenham untuk rendering garis
- Implementasi algoritma DDA untuk plotting garis
- Implementasi algoritma Midpoint untuk rendering lingkaran dan elips
- Pengembangan modul transformasi (translasi, rotasi, dan scaling)

### Tahap 3: Pengembangan Objek Statis (30 Maret - 5 April)
- Implementasi Rumah Honai dalam mode kerangka
- Implementasi Tas Noken dalam mode kerangka
- Implementasi Sate Ulat Sagu dalam mode kerangka
- Pembuatan sistem koordinat Kartesius untuk penempatan objek

### Tahap 4: Pengembangan Objek Berwarna (6 April - 12 April)
- Penambahan teknik pengisian warna pada objek Rumah Honai
- Penambahan teknik pengisian warna pada objek Tas Noken
- Penambahan teknik pengisian warna pada objek Sate Ulat Sagu
- Implementasi motif-motif khusus pada objek (zigzag, garis-garis, dll)

### Tahap 5: Animasi dan Interaksi (13 April - 19 April)
- Implementasi animasi pergerakan objek menggunakan teknik interpolasi
- Pembuatan menu navigasi antar karya
- Penambahan interaksi pengguna melalui tombol-tombol kontrol
- Pengembangan alur cerita dalam bentuk storyboard animasi

### Tahap 6: Finalisasi dan Pengujian (20 April - 22 April)
- Perbaikan bug dan optimasi performa
- Pengujian kompatibilitas
- Penyusunan dokumentasi kode
- Finalisasi proyek dan persiapan demonstrasi

## Teknologi yang Digunakan
- Godot Engine 3.x
- Bahasa C# untuk implementasi algoritma dan logika
- Pemrograman grafika primitif tanpa memanfaatkan fungsi bawaan engine

## Cara Menggunakan
1. Buka proyek dengan Godot Engine 3.x
2. Jalankan scene utama dari menu
3. Gunakan tombol navigasi untuk beralih antar mode tampilan dan animasi

## Catatan Pengembangan
Proyek ini dikembangkan sebagai bagian dari tugas mata kuliah Komputer Grafik dengan fokus pada implementasi manual algoritma grafika dasar, tanpa menggunakan fungsi rendering bawaan dari Godot Engine.
