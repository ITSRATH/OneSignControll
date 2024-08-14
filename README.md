# OneSignControll

OneSignControll ist eine Windows-Anwendung, die es Benutzern ermöglicht, 
mehrere Programme und Microsoft Management Console (MMC)-Dateien an einem zentralen Ort zu verwalten und mit erhöhten Rechten auszuführen. 
Die Anwendung unterstützt das Laden und Speichern von Konfigurationen in einer XML-Datei, 
die im AppData-Ordner des Benutzers gespeichert wird.

## Funktionen

- **Unterstützung von `.exe`, `.msi`, `.bat`, `.cmd`, `.vbs`, `.ps1`, `.msc`, und `.cpl` Dateien:**
  - Diese Dateien können in der Anwendung hinzugefügt und mit Administratorrechten ausgeführt werden.
  
- **Multiselect-Funktionalität:**
  - Benutzer können mehrere Programme gleichzeitig auswählen und ausführen.

- **XML-Konfigurationsmanagement:**
  - Speichern und Laden von Programmlisten in einer XML-Datei im AppData-Ordner des Benutzers.
  
- **Import/Export-Funktionalität:**
  - Benutzer können Programmlisten von einer XML-Datei importieren oder in eine exportieren.

- **Nur Deutsch unterstützt:**
  - Die Benutzeroberfläche und alle Funktionen sind ausschließlich in deutscher Sprache verfügbar.

## Installation

Um OneSignControll zu installieren und zu verwenden:

1. Laden Sie die neueste Version von der [Releases-Seite](#) herunter.
2. Führen Sie `OneSignControll.exe` aus. Es ist keine Installation erforderlich.

## Verwendung

### Programme hinzufügen

1. Klicken Sie auf `Programme` > `Programm Hinzufügen`.
2. Wählen Sie eine `.exe`, `.msi`, `.bat`, `.cmd`, `.vbs`, `.ps1`, `.msc` oder `.cpl`-Datei aus.
3. Bestätigen Sie den Namen des Programms oder passen Sie ihn an.

### Programme umbenennen

1. Wählen Sie ein Programm in der Liste aus.
2. Klicken Sie auf `Programme` > `Programm Umbennennen`.
3. Geben Sie den neuen Namen ein und bestätigen Sie.

### Programme entfernen

1. Wählen Sie ein oder mehrere Programme in der Liste aus.
2. Klicken Sie auf `Programme` > `Programm Entfernen`.

### XML-Konfiguration speichern/laden

- Um Ihre aktuelle Programmliste zu speichern, klicken Sie auf `Datei` > `Speicher Konfiguration`.
- Um eine Programmliste zu laden, klicken Sie auf `Datei` > `Lade Konfiguration`.

### Import/Export

- Um eine Programmliste zu importieren, klicken Sie auf `Datei` > `Importiere XML`.
- Um eine Programmliste zu exportieren, klicken Sie auf `Datei` > `Exportiere XML`.

## Anforderungen

- **Windows 7 oder höher**
- **.NET Framework 4.8**

## Fehlersuche

### Problem: "Windows cannot access the specified device, path, or file" Fehler

- Stellen Sie sicher, dass Sie die Datei mit Administratorrechten ausführen.
- Überprüfen Sie die Dateiberechtigungen und Gruppenrichtlinien, die den Zugriff auf die Datei einschränken könnten.

### Problem: Programme werden nicht korrekt ausgeführt

- Überprüfen Sie, ob die richtigen Dateitypen hinzugefügt wurden.
- Stellen Sie sicher, dass die ausgewählten Programme Administratorrechte benötigen, um korrekt ausgeführt zu werden.

## Mitwirkende

- Patrick Rath - Hauptentwickler und Autor.

## Lizenz

Dieses Projekt steht unter der MIT-Lizenz.

## Danksagungen

- Vielen Dank an alle, die zur Entwicklung und Verbesserung dieses Projekts beigetragen haben.
