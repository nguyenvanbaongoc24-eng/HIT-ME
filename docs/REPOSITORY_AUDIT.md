# HIT ME — Audit trước Unity foundation

Ngày: 08/10/2026. Giai đoạn 1: audit, đọc spec, đề xuất cấu trúc. Dừng trước foundation theo master prompt.

## Hiện trạng và kiểm chứng

- Workspace chưa phải Git repository (`git status` báo không có `.git`).
- Có monorepo TypeScript: `apps/client` dùng Phaser/Vite, `packages/game-core` chứa logic thuần, `packages/shared-config` chứa cấu hình.
- Chưa có `unity-client`, server, Unity scenes, ProjectSettings hoặc Packages manifest của Unity.
- Có Unity Hub trong Program Files; chưa tìm thấy Editor qua PATH hoặc đường dẫn mặc định `C:/Program Files/Unity/Hub/Editor`. Đây không phải kết luận rằng Editor không tồn tại ở mọi đường dẫn tùy chỉnh.
- Có .NET CLI. Chưa chạy C# test, Unity import hay Unity Web build.
- `npm test`: TypeScript build thành công, **9/9 test game-core qua**. Bao gồm hit, xuyên tối đa một người theo config hiện tại, sát thương đồng thời, cùng chết, ray elip ngang/dọc/chéo, kẹp vị trí và hai mục tiêu thẳng hàng. Chưa có test riêng mục tiêu phía sau trên elip hoặc chuyển đổi tọa độ/responsive.
- Manifest hiện có một nhân vật với sáu trạng thái nhưng mọi danh sách frame rỗng. Các SVG đang có là asset prototype, không phải bộ sprite art hoàn chỉnh cho Unity.
- Chưa kiểm tra UI bằng browser, điện thoại, FPS hoặc context loss trong giai đoạn audit.

## Nguồn yêu cầu và ưu tiên

Đã đọc ba tài liệu tại root và chép nguyên bản vào `docs/` theo đường dẫn master prompt yêu cầu. Không thiếu ba spec này.

1. Master prompt hiện tại chốt Unity 6, 2D URP, Web trước, 1 người và 2 bot cho vertical slice.
2. Prompt 1C thay Prompt 1B: elip, tọa độ chân, manifest, HUD nổi, placeholder có nhãn.
3. Spec gốc cung cấp luật; các mục đề xuất/cần xác nhận không tự trở thành luật đã chốt.
4. UI prompts dùng cho thao tác và trình tự hiển thị khi không mâu thuẫn với hai nguồn trên.

Không áp dụng stack Phaser/Capacitor/Colyseus đề xuất trong spec vào Unity. Giữ prototype hiện có để tham khảo và đối chiếu test. Tài liệu 1D có tại root nhưng master prompt không chỉ định nó thay thế 1C; chưa dùng làm yêu cầu triển khai.

## File tree đề xuất cho các giai đoạn tiếp theo

```text
AGENTS.md
README.md
unity-client/
  Assets/HitMe/
    Scripts/
      Core/          # C# thuần: model, config, phase machine, offline authority
      Arena/         # Hình học thuần + adapter hiển thị
      Characters/    # Manifest, giới tính, slot mở rộng, sáu trạng thái
      Combat/        # Interface và logic đã được xác nhận
      Networking/    # DTO, transport interface, mock
      UI/            # HUD, safe area, input, loading, quality
      Audio/         # Hook SFX
      Localization/  # vi/en
    Tests/EditMode/
    Scenes/          # Boot, MainMenu, Lobby, CharacterSelect, Battle, Result
    Resources/       # Config và manifest tối thiểu; không nạp mọi bản đồ
    Art/             # Placeholder, font, Sprite Atlas và giấy phép
    Editor/          # Tạo scene/config và build Web
  Packages/manifest.json
  ProjectSettings/
server/
  src/               # Node/TypeScript: interface authority, validation; chưa WSS thật
shared/
  protocol/          # Version, message schema, dữ liệu công khai/riêng
  config/            # Nguồn cấu hình + kiểm tra đồng bộ C#/TS
tests/               # Test protocol, golden vectors đối chiếu TS/C#
tools/               # Scene/build/config tooling
docs/
  REPOSITORY_AUDIT.md
  MISSING_REQUIREMENTS.md
  MULTIPLAYER_ARCHITECTURE.md
  UNITY_EDITOR_GUIDE.md
  ACCEPTANCE_CHECKLIST.md
apps/                # Prototype hiện có
packages/            # Logic TS hiện có
```

## Trình tự sau audit

- Foundation: project Unity 6 URP và cấu hình có version xác định từ Editor thực tế, module/assembly, i18n, cấu hình, geometry và EditMode tests. Báo cáo rồi dừng.
- Vertical slice: Battle, một người + hai bot, timer/ready, placement/aim/reveal và state machine. Chỉ bật resolve khi chính sách xuyên mục tiêu đã rõ; các quy tắc chưa chốt có thể được biểu diễn bằng interface/chế độ chờ xác nhận. Báo cáo rồi dừng.
- Bàn giao: protocol/mock, tài liệu Web build và checklist; kết quả build/test chỉ ghi khi thực sự chạy.

## File đã tạo ở giai đoạn này

- Ba bản sao spec trong `docs/`.
- `docs/REPOSITORY_AUDIT.md`.
- `docs/MISSING_REQUIREMENTS.md`.

Lệnh test đã tái tạo output `packages/*/dist` bằng compiler hiện có; không sửa source prototype hay luật. Không có Git nên chưa thể đối chiếu diff với baseline.
