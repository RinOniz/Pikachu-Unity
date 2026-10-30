# PIKACHU MODDED (2D)

> **Dự án Môn học**: Thiết Kế Trò Chơi (Game Design) - UTC  
> **Nền tảng**: Unity 2D (C#)

---

## 1. Thông Tin Đề Tài & Tác Giả

* **Tên đề tài**: Pikachu Modded
* **Thể loại**: Giải đố Logic (Puzzle)
* **Công cụ & Công nghệ phát triển**:
  - Engine: Unity 6.0
  - Ngôn ngữ: C#
  - Quản lý phiên bản: Git & GitHub
* **Thành viên thực hiện**:
  - **Họ và tên**: Chu Nguyễn Khánh Thiện
  - **Vai trò**: Độc lập phát triển toàn bộ dự án (Game Designer, Programmer, UI/UX Integrator, Sound & Polish).

---

## 2. Giới Thiệu Tổng Quát Dự Án

**Pikachu Onet Connect** là dự án game giải đố 2D được xây dựng lại từ trò chơi nối thú cổ điển huyền thoại (Pikachu Onet 2003), đồng thời được nghiên cứu mở rộng và tích hợp thêm nhiều cơ chế gameplay hiện đại, mang lại trải nghiệm vừa quen thuộc vừa mới lạ cho người chơi.

Trò chơi bao gồm thuật toán tìm đường Onet chuẩn xác, hệ thống tính điểm, hiệu ứng vẽ đường nối (`LineRenderer`) trực quan, âm thanh sống động và 5 chế độ chơi độc đáo:

```
[ Menu Chính ]
  ├── 1. Classic Mode 
  ├── 2. Funny Mode 
  └── 3. Special Modes 
        ├── Memory Mode 
        ├── Tetris Falling Mode 
        └── Piano Tiles Mode 
```

---

## 3. Chi Tiết Các Chế Độ Chơi (Game Modes)

### 3.1. Chế độ Cổ điển (Classic Mode)
* **Luật chơi**: Tìm và nối 2 ô Pokemon giống nhau sao cho đường đi giữa chúng không quá 2 khúc gấp (thuật toán tìm đường I, L, Z/U).
* **Độ khó tăng dần theo Level**:
  - Các cấp độ áp dụng quy luật dồn ô khác nhau sau mỗi lần ăn (đứng yên, rơi xuống đáy, dồn lên đỉnh).
* **Công cụ hỗ trợ**: 
  - `Hint`: Tìm và làm sáng cặp ô có thể ăn được.
  - `Change`: Xáo trộn lại vị trí toàn bộ bàn cờ khi bị bế tắc hoặc muốn đổi thế cờ.
  - `Pause`: Tạm dừng trò chơi.

### 3.2. Chế độ Vui vẻ (Funny Mode)
* Mang tính ngẫu hứng và bất ngờ: Mỗi khi người chơi nối thành công một cặp thú bất kỳ, bàn cờ sẽ **tự động xáo trộn ngẫu nhiên** toàn bộ các quân cờ còn lại.
* Đòi hỏi khả năng quan sát nhanh và thích ứng liên tục với bàn cờ mới.

### 3.3. Chế độ Ghi nhớ (Memory Mode)
* Toàn bộ các quân cờ đều bị **úp mặt sau**.
* **Cơ chế ghi nhớ nhịp điệu**:
  - Khi bắt đầu màn: Người chơi có **5 giây** để nhìn và ghi nhớ toàn bộ vị trí các Pokemon trước khi úp xuống.
  - Trong quá trình chơi: Cứ sau mỗi **5 giây** úp bài, hệ thống sẽ tự động mở hé toàn bộ các ô trong **2 giây** để người chơi kịp thời quan sát lại và định hình vị trí các cặp thú cần nối.

### 3.4. Chế độ Tetris Sinh tồn (Tetris Falling Mode)
* Bàn cờ biến động không ngừng theo thời gian:
  - Cứ sau mỗi chu kỳ thời gian (đếm ngược 10 giây thông báo trên màn hình), một hàng Pokemon mới sẽ được sinh ra và đẩy từ dưới đáy lên.
  - Người chơi phải liên tục tìm và triệt tiêu các cặp thú để hạ thấp độ cao bàn cờ.
  - Nếu các khối gạch chạm vạch đỏ trên cùng $\rightarrow$ **Game Over**.
* Tích hợp 2 tùy chọn trọng lực:
  - **No Gravity**: Các quân cờ sau khi ăn sẽ để lại khoảng trống cố định.
  - **Gravity**: Các quân cờ phía trên tự động rơi xuống lấp đầy các khoảng trống bên dưới.

### 3.5. Chế độ Piano Tiles (Piano Pokemon Mode)
* Lối chơi Arcade phản xạ âm nhạc kết hợp 12 phím bấm:
  - Bàn cờ chia làm 3 khu vực:
    - **Bên trái (6 ô reference)**: 6 loài Pokemon tương ứng với các phím `Q, W, E` (hàng trên) và `A, S, D` (hàng dưới).
    - **Bên phải (6 ô reference)**: 6 loài Pokemon tương ứng với các phím `U, I, O` (hàng trên) và `J, K, L` (hàng dưới).
    - **Trung tâm (4 làn rơi)**: Các icon Pokemon rơi tự do với các vạch phân làn và vạch phán định (Hit Line) phát sáng ở đáy.
  - **Cơ chế**: Khi nốt rơi đến vạch đệm đáy, người chơi nhấn phím tương ứng của loài Pokemon đó để ghi điểm (+100 điểm, thẻ chớp xanh).
  - Tốc độ rơi tăng dần theo chuỗi combo thành công.
  - Người chơi có **3 Mạng sống (Lives)**; bấm sai hoặc để nốt trôi qua đáy sẽ bị mất 1 mạng.

---

## 4. Cấu Trúc Thư Mục Dự Án (Project Structure)

```text
Assets/
├── Prefabs/             # Prefab ô cờ (Tile, MemoryTile, ...)
├── Resources/           # Tài nguyên load động (Pokemon sprites, Audios, UI icons)
├── Scenes/              # Các phân cảnh trò chơi
│   ├── MenuScene.unity        # Giao diện chọn chế độ chính
│   ├── GameScene.unity        # Màn chơi Classic & Funny
│   ├── MemoryGameScene.unity  # Màn chơi Memory Mode
│   ├── EndlessGameScene.unity # Màn chơi Tetris Falling Mode
│   └── PianoGameScene.unity   # Màn chơi Piano Tiles Mode
└── Scripts/             # Mã nguồn C#
    ├── Core/            # Thuật toán tìm đường, Board, Tile
    ├── Features/        # Tính năng Hint, Change, Pause, ReturnToMenu
    ├── Manager/         # GameController, LevelSelect, ModeSelect
    ├── Memory/          # Logic chế độ Memory
    ├── Endless/         # Logic chế độ Tetris Falling
    ├── Piano/           # Logic chế độ Piano Tiles
    └── UI/              # ResultPanel, SpecialModePanel, HUD
```

---

## 5. Hướng Dẫn Cài Đặt & Trải Nghiệm

1. **Mở dự án trong Unity**:
   - Sử dụng Unity Editor phiên bản tương thích (6000.0.73f1 LTS).
   - Mở thư mục gốc `Pikachu`.
   - Trong cửa sổ `Project`, điều hướng đến `Assets/Scenes/MenuScene.unity`.
   - Nhấn nút **Play** trên thanh công cụ Unity để trải nghiệm đầy đủ tất cả các chế độ chơi.
2. **Chơi game trên nền tảng WebGL**:
   - Truy cập đường link: https://rinoniz.itch.io/onet-connect-modded
3. **Chơi game trên Windows**:
   - Tải toàn bộ source project hoặc tải folder BuildWindows, mở Pikachu.exe để chơi.

