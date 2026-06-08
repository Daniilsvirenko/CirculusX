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
