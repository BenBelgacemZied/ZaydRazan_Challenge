# ZR LingoTrip

**ZR LingoTrip** est une application Android éducative pour les enfants de 8 à 12 ans. Chaque
destination fixe la langue apprise. À Paris, la langue cible reste le français ; la langue de départ
de l'enfant forme l'autre moitié de la paire (actuellement néerlandais → français).

## Architecture

La solution est séparée en projets avec des dépendances orientées vers le cœur :

- `ZRLingoTrip.Domain` : modèles de destination, paire linguistique, puzzles et vocabulaire ; aucune dépendance MAUI.
- `ZRLingoTrip.Application` : contrats et comportements partagés, dont les catalogues et les ports de progression et de synthèse vocale.
- `ZRLingoTrip.Content.Paris` : contenu de Paris pour la paire néerlandais → français.
- `ZRLingoTrip.Infrastructure.Maui` : adaptateurs MAUI pour la persistance locale et la synthèse vocale.
- `ZaydRazanChallenge` : interface MAUI, pages organisées par fonctionnalité, navigation et composition des dépendances dans `MauiProgram`.

Les pages reçoivent leurs dépendances par constructeur. Les ressources visuelles restent dans le projet MAUI,
classées sous `Resources/Images/Adventure`, `Paris`, `Puzzles` et `Shared` ; leur nom logique est aplati
au packaging pour garder les références d'images existantes.

La structure de la paire linguistique est générique (`source` → `cible`), mais le seul contenu fourni pour
l'instant est néerlandais → français. Une autre langue de départ demandera son jeu de contenus Paris traduit.

## Première destination : Paris

- **Woorden verbinden** : associer un mot français à sa traduction néerlandaise.
- **Luisteren en kiezen** : écouter une prononciation française et choisir.
- **Monumentenquiz** : découvrir la tour Eiffel, le Louvre, Notre-Dame,
  l'Arc de Triomphe et le Sacré-Cœur.
- Étoiles et progression sauvegardées localement, sans compte.

## Démarrage

Prérequis : .NET 10 SDK, workload MAUI Android et Android SDK.

```bash
dotnet workload install maui-android
dotnet build ZRLingoTrip.slnx
```

Ouvrez ensuite le projet dans Visual Studio 2022 et sélectionnez un émulateur
Android ou un téléphone connecté.
