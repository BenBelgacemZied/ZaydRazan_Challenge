# Publication Google Play — ZR LingoTrip 2.37

Ce dossier contient les textes et les réponses préparées pour Play Console.

## Fichier à téléverser

- Format : Android App Bundle (`.aab`)
- Package immuable : `com.zrlingotrip.app`
- Version : `2.37` (`versionCode` 48)
- Cible : Android 16 / API 36
- Signature : clé d'upload Android déjà configurée dans GitHub Actions

## Ordre de publication conseillé

1. Créer l'application **ZR LingoTrip** dans Play Console.
2. Choisir le néerlandais comme langue par défaut et la catégorie **Éducation**.
3. Copier les textes de `listings/nl-NL` et ajouter la traduction française de `listings/fr-FR`.
4. Ajouter l'icône 512 × 512, l'image de présentation 1024 × 500 et les captures d'écran téléphone.
5. Publier `docs/privacy-policy.html` sur une URL HTTPS publique, puis saisir cette URL dans Play Console.
6. Remplir les formulaires selon `console-declarations.md`.
7. Créer une version en **test interne**, téléverser l'AAB et tester l'installation depuis Google Play.
8. Corriger les alertes du tableau de bord, puis promouvoir la même version vers la production.

Ne jamais créer une nouvelle clé d'upload pour une mise à jour : conserver les secrets Android déjà configurés.
