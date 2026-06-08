# Инструкция по интеграции и настройке сцены Hotel Hallway

В этом документе описано, как настроить импортированную 3D-модель коридора отеля (**Hotel Hallway**), настроить материалы (включая ковер и стены) и быстро расставить источники света.

*Deutschsprachige Anleitung weiter unten.*

---

## 1. Структура файлов в проекте
* **[hallway_hotel.fbx](file:///Assets/HotelHallway/hallway_hotel.fbx)** — 3D-модель коридора.
* **[tex/](file:///Assets/HotelHallway/tex/)** — Текстуры для материалов.
* **[AutoPlaceLights.cs](file:///Assets/Editor/AutoPlaceLights.cs)** — Редакторский скрипт для автоматической расстановки источников света.
* **[AutoAssignTextures.cs](file:///Assets/Editor/AutoAssignTextures.cs)** — Редакторский скрипт для автоматической привязки текстур.
* **[Materiials/](file:///Assets/Materiials/)** — Извлеченные материалы сцены.

---

## 2. Базовые настройки импорта модели (FBX)
При выборе файла `hallway_hotel` в окне *Project*, в окне *Inspector* должны быть выставлены следующие параметры:
1. **Вкладка Model**:
   * **Scale Factor**: `1` (при необходимости скорректируйте масштаб).
   * **Generate Colliders**: Должна стоять галочка (чтобы игрок не проваливался сквозь пол и не проходил сквозь стены).
   * **Generate Lightmap UVs**: Должна стоять галочка (необходимо для качественного запекания света). Нажмите **Apply**.
2. **Вкладка Materials**:
   * Материалы уже извлечены в папку `Assets/Materiials/`. Если вы хотите извлечь их заново, нажмите кнопку **Extract Materials...** и укажите папку.

---

## 3. Настройка материалов и замена текстур
Проект использует **URP (Universal Render Pipeline)**. Все материалы используют шейдер `Universal Render Pipeline/Lit`.

### Автоматическая привязка текстур (Рекомендуется)
Так как извлеченные материалы по умолчанию не привязаны к текстурам и выглядят как однотонные цветные шары:
1. В верхнем меню Unity нажмите: **`Tools` $\rightarrow$ `Auto Assign Textures to Materials`**.
2. Скрипт сам привяжет Albedo и Normal карты для ковра (`carpet`), столов (`tables`), стен (`walls`), металлических частей (`metal_lights`) и дерева дверей (`door_wood`), а также сбросит цветные тинты на белый цвет и включит Emission для плафонов (`light_cover`).

### Изменение материала стен (если не нравится стандартный)
* **Вариант А (Другая текстура)**: Выберите материал `walls` в папке `Assets/Materiials/` и перетащите в слот **Base Map** другую текстуру (например, деревянную текстуру `TCom_Wood_MahoganyVeneer_1K_albedo_lite` для эффекта деревянных панелей).
* **Вариант Б (Стильный однотонный матовый цвет)**: Удалите текстуру из слота **Base Map** у материала `walls` (нажмите Backspace/Delete на текстуре). Выберите любой цвет в поле **Base Color** (например, темно-серый графит или кремовый). Текстура рельефа штукатурки `TCom_Wall_Stucco6A_2x2_1K_normal` при этом останется в слоте Normal Map.

---

## 4. Автоматическая расстановка источников света
Для быстрой расстановки ламп используется редакторский скрипт `AutoPlaceLights`:
1. Откройте сцену и найдите объект модели `hallway_hotel`.
2. В иерархии сцены раскройте модель и выделите родительский объект **`lights`** (содержит объекты от `0` до `27`).
3. В верхнем меню Unity нажмите: **`Tools` $\rightarrow$ `Place Lights on Fixtures`**.
4. Скрипт автоматически создаст под каждым плафоном точечный источник света (`Point Light`) с мягкими тенями.
5. Проделайте то же самое для объекта **`_1`** (содержит плафон `lights_2`).

> [!NOTE]
> Смещение ламп настроено по умолчанию на локальные координаты `(0, 0.033, 0.106)`. Если вам потребуется изменить высоту или смещение источников света для новых генераций, откройте скрипт `AutoPlaceLights.cs` и измените значения в строке:
> ```csharp
> lightGo.transform.localPosition = new Vector3(0f, 0.033f, 0.106f);
> ```

---

## 5. Эмиссия плафонов и Запекание света
Чтобы плафоны ламп визуально светились:
1. Выберите материал **`light_cover`** в папке `Assets/Materiials/`.
2. Включите галочку **Emission**.
3. Установите теплый желтый цвет и в режиме HDR увеличьте интенсивность свечения (**Intensity**) до `1.5`–`2.0`.
4. В поле *Global Illumination* выберите **Baked**.
5. Убедитесь, что все объекты коридора на сцене отмечены галочкой **Static** в правом верхнем углу инспектора.
6. Откройте окно запекания света через **`Window` $\rightarrow$ `Rendering` $\rightarrow$ `Lighting`** и нажмите **`Generate Lighting`**.

---
---

# Integrations- und Einrichtungsanleitung für die Hotel-Flur-Szene (Hotel Hallway)

Diese Dokumentation beschreibt, wie das importierte 3D-Modell des Hotelflurs (**Hotel Hallway**) eingerichtet wird, wie die Materialien (einschließlich Teppich und Wände) konfiguriert werden und wie die Lichtquellen schnell platziert werden können.

---

## 1. Dateistruktur im Projekt
* **[hallway_hotel.fbx](file:///Assets/HotelHallway/hallway_hotel.fbx)** – Das 3D-Modell des Flurs.
* **[tex/](file:///Assets/HotelHallway/tex/)** – Texturen für die Materialien.
* **[AutoPlaceLights.cs](file:///Assets/Editor/AutoPlaceLights.cs)** – Editor-Skript zur automatischen Platzierung der Lichtquellen.
* **[AutoAssignTextures.cs](file:///Assets/Editor/AutoAssignTextures.cs)** – Editor-Skript zur automatischen Zuweisung von Texturen zu Materialien.
* **[Materiials/](file:///Assets/Materiials/)** – Extrahierte Materialien der Szene.

---

## 2. Grundlegende Importeinstellungen für das Modell (FBX)
Wenn Sie die Datei `hallway_hotel` im *Project*-Fenster auswählen, sollten im *Inspector*-Fenster folgende Parameter eingestellt sein:
1. **Reiter Model**:
   * **Scale Factor**: `1` (bei Bedarf anpassen).
   * **Generate Colliders**: Aktivieren Sie dieses Kontrollkästchen (damit der Spieler nicht durch den Boden fällt oder durch Wände geht).
   * **Generate Lightmap UVs**: Aktivieren Sie dieses Kontrollkästchen (erforderlich für das Backen von Licht/Lightmapping). Klicken Sie unten auf **Apply**.
2. **Reiter Materials**:
   * Die Materialien wurden bereits in den Ordner `Assets/Materiials/` extrahiert. Wenn Sie sie erneut extrahieren möchten, klicken Sie auf **Extract Materials...** und wählen Sie den Ordner aus.

---

## 3. Zuweisung von Texturen (Automatisch & Manuell)
Das Projekt verwendet die **URP (Universal Render Pipeline)**. Alle Materialien verwenden den Shader `Universal Render Pipeline/Lit`.

### Automatische Einrichtung (Empfohlen)
Da die Texturen nach dem Extrahieren nicht automatisch zugewiesen werden und die Materialien nur als einfarbige Kugeln erscheinen, wurde ein Hilfsskript erstellt:
1. Klicken Sie im oberen Unity-Menü auf: **`Tools` $\rightarrow$ `Auto Assign Textures to Materials`**.
2. Das Skript verknüpft automatisch die Albedo- und Normal-Maps für den Teppich (`carpet`), die Tische (`tables`), die Wände (`walls`), die Lampen-Metallteile (`metal_lights`) und das Türholz (`door_wood`), setzt die Farbtönungen (Color Tint) auf Weiß zurück und aktiviert die Emission für die Lampenabdeckungen (`light_cover`).

### Manueller Wechsel der Wandmaterialien
Wenn Ihnen das Aussehen der Wände nicht gefällt:
* **Option A (Textur ändern)**: Wählen Sie das Material `walls` in `Assets/Materiials/` aus und ziehen Sie eine andere Textur in das Feld **Base Map** (z. B. `TCom_Wood_MahoganyVeneer_1K_albedo_lite` für Holzpaneele).
* **Option B (Einfarbig matt)**: Löschen Sie die Textur aus dem Feld **Base Map** des `walls`-Materials (drücken Sie Backspace auf der Textur). Wählen Sie eine gewünschte Farbe im Feld **Base Color** (z. B. Anthrazit oder Cremeweiß). Belassen Sie die Normal-Map `TCom_Wall_Stucco6A_2x2_1K_normal` im entsprechenden Feld für eine Putz-Struktur.

---

## 4. Automatische Platzierung der Lichtquellen
So platzieren Sie die Lichter im Flur automatisch:
1. Wählen Sie in der Hierarchie der Szene das übergeordnete Objekt **`lights`** aus (enthält die Objekte `0` bis `27`).
2. Klicken Sie im oberen Unity-Menü auf: **`Tools` $\rightarrow$ `Place Lights on Fixtures`**.
3. Das Skript erstellt unter jeder Lampe ein `Point Light` mit warmem Licht und weichen Schatten an den lokalen Koordinaten `(0, 0.033, 0.106)`.
4. Wiederholen Sie den Vorgang für das Objekt **`_1`** (enthält `lights_2`).

---

## 5. Emission der Lampen und Backen des Lichts (Lightmapping)
Damit die Lampenschirme leuchten:
1. Wählen Sie das Material **`light_cover`** in `Assets/Materiials/` aus.
2. Aktivieren Sie das Kontrollkästchen **Emission**.
3. Wählen Sie eine warme gelbe HDR-Farbe und erhöhen Sie die Intensität auf `1.5` bis `2.0`.
4. Stellen Sie *Global Illumination* auf **Baked**.
5. Stellen Sie sicher, dass alle Flurobjekte in der Szene oben rechts im Inspector als **Static** markiert sind.
6. Öffnen Sie das Lichtfenster über **`Window` $\rightarrow$ `Rendering` $\rightarrow$ `Lighting`** und klicken Sie auf **`Generate Lighting`**.
