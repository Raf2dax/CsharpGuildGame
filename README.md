# Guild Manager - Jeu de Gestion de Guilde en C#

Un jeu de stratégie tour par tour en C# où vous gérez une guilde d'aventuriers, planifiez des missions, gérez les ressources et entraînez vos héros.

## 📋 Table des matières

- [Aperçu](#aperçu)
- [Fonctionnalités](#fonctionnalités)
- [Prérequis](#prérequis)
- [Installation](#installation)
- [Comment jouer](#comment-jouer)
- [Structure du projet](#structure-du-projet)
- [Mécanique de jeu](#mécanique-de-jeu)

## 🎮 Aperçu

Guild Manager est un jeu de gestion où vous dirigez une petite guilde d'aventuriers. Chaque jour est divisé en trois phases (Matin, Après-midi, Soir). Vous devez assigner vos héros à des missions, acheter des ressources auprès du marchand, et gérer leur santé et fatigue pour garder votre guilde à flot.

## ✨ Fonctionnalités

### Système de Héros
- **7 classes uniques** avec stats différentes : Guerrier, Voleur, Mage, Rôdeur, Clerc, Paladin, Berserker
- **Traits héréditaires** : Brave, Chanceux, Malchanceux, etc.
- **Système de progression** avec niveaux et salaires
- **3 héros de départ** : Corin (Guerrier), Elara (Mage), Thaddeus (Clerc)
- **Recrutement** de nouveaux héros aléatoires avec délais d'arrivée

### Système de Missions
- **6 types de missions** avec difficultés variables (1-5)
- **Récompenses dynamiques** : or, nourriture, équipement
- **Objets rares** avec chances de drop (0-20%)
- **Événements aléatoires** : rencontre de héros, embuscades, ressources bonus
- **Durée variable** : 1-2 phases de mission

### Gestion des Ressources
- **Or** : Payez les salaires, achetez des ressources
- **Nourriture** : Nourrir vos héros chaque matin
- **Soins** : Guérir les blessures après les missions
- **Équipement** : Augmenter la puissance en combat
- **Dette** : Accumulée si vous manquez de ressources

### Système de Combat
- **Calcul de puissance** basé sur : niveau, équipement, traits, classes préférées
- **Formule de succès** : 55% + (puissance - difficulté*1.5) * 5%
- **Blessures** : Héros blessés après échecs
- **Fatigue** : Augmente après les missions
- **Faim** : Nécessite de la nourriture chaque matin

### Interface
- **Affichage clair** de la phase actuelle (Matin/Après-midi/Soir)
- **Fenêtre Marchand** pour acheter des ressources
- **Fenêtre de Recrutement** avec 5 héros aléatoires
- **Journal détaillé** de tous les événements
- **Bouton Recommencer** après une défaite
- **Affichage du statut** des héros (Libre/Occupé, ArrivingDay)

## 📦 Prérequis

- **.NET 8.0** ou supérieur
- **Visual Studio 2022** ou VS Code avec C# Dev Kit
- **Windows** (application WPF)

## 🚀 Installation

1. **Cloner ou télécharger** le projet
2. **Ouvrir la solution**
   ```bash
   cd "C:\Users\[username]\Desktop\Cours\C#"
   dotnet sln open C#.sln
   ```
3. **Construire le projet**
   ```bash
   dotnet build C#.sln
   ```
4. **Lancer le jeu**
   ```bash
   dotnet run --project GuildGame\UI\UI.csproj
   ```

## 🎯 Comment jouer

### Démarrage
Le jeu démarre automatiquement en **Phase du Matin** du jour 1.

### Cycle Quotidien
1. **Matin** : Distribution de nourriture, paiement des salaires, arrivée des nouveaux héros
2. **Après-midi** : Assignation et exécution des missions
3. **Soir** : Résolution finale, événements aléatoires

### Actions principales

**Gérer vos Héros**
- Sélectionnez un héros pour voir ses stats
- Visualisez PV, Fatigue, Faim, Niveau
- Voyez son statut (Libre ou Occupé)

**Assigner des Missions**
- Cliquez sur une mission disponible
- Choisissez les héros à envoyer
- La mission disparaît de la liste après assignation
- Cliquez sur "Annuler" pour revenir en arrière

**Acheter au Marchand**
- Cliquez sur le bouton "Marchand"
- Achetez : Pack de nourriture (6 or), Trousse de soins (5 or), Armes solides (8 or)

**Recruter des Héros**
- Cliquez sur "Recruter un héros"
- Sélectionnez parmi 5 héros aléatoires
- Les héros arrivent le jour suivant
- La fenêtre reste ouverte pour recruter plusieurs héros

**Équiper vos Héros**
- Cliquez sur un équipement dans l'inventaire
- Affectez-le au héros sélectionné
- Max 3 équipements par héros

### Condition de Victoire
Pas de limite - jouez aussi longtemps que vous le souhaitez!

### Condition de Défaite
Perdez si **l'un des cas se produit** :
- Tous les héros sont morts
- Vous ne pouvez pas payer les salaires
- Vous ne pouvez pas nourrir vos héros vivants

Cliquez sur "Recommencer" pour relancer une nouvelle partie.

## 🏗️ Structure du projet

```
GuildGame/
├── Domain/                    # Modèles métier
│   └── Models/
│       ├── Hero.cs           # Héros avec stats et équipement
│       ├── Mission.cs        # Missions avec récompenses
│       ├── RareItem.cs       # Objets rares et buffs
│       ├── RandomEvent.cs    # Événements aléatoires
│       ├── GuildState.cs     # État global de la guilde
│       ├── HeroClass.cs      # Énumération des classes
│       └── Trait.cs          # Traits héréditaires
│
├── Services/                  # Logique métier
│   ├── GameEngine.cs         # Boucle de jeu principale
│   ├── MissionResolver.cs    # Résolution des missions
│   ├── EventResolver.cs      # Génération des événements
│   ├── MerchantService.cs    # Offres du marchand
│   └── ContentFactory.cs     # Génération de contenu
│
└── UI/                        # Interface WPF
    ├── MainWindow.xaml       # Fenêtre principale
    ├── MerchantWindow.xaml   # Fenêtre marchand
    ├── RecruitmentWindow.xaml# Fenêtre recrutement
    ├── ViewModels/
    │   └── GameViewModel.cs  # Logique UI
    ├── Converters/
    │   ├── AbsValueConverter.cs        # Affichage valeurs absolues
    │   └── StringToVisibilityConverter.cs # Visibilité conditionnelle
    └── Infrastructure/
        └── ObservableObject.cs  # Base MVVM
```

## 🎲 Mécanique de jeu

### Calcul du Succès de Mission
```
Puissance = Niveau des héros + Nombre de héros + Bonus équipement
Puissance += Bonus traits (Brave +2, Chanceux +2, Malchanceux -1)
Puissance += Bonus classes préférées

Difficulté = Difficulté de mission × 1.5
Chance = 55% + (Puissance - Difficulté) × 5%
Chance = Limité entre 10% et 95%
```

### Événements Aléatoires (15% de chance par mission réussie)
- **Rencontre** : +1 héros aléatoire
- **Embuscade** : -5 à -15 PV aux héros
- **Découverte** : +20 à +50 or, +10 à +25 nourriture

### Classes et Stats

| Classe | PV | Fatigue | Salaire |
|--------|----|---------|---------| 
| Guerrier | 110 | 15 | 5 |
| Voleur | 90 | 10 | 4 |
| Mage | 80 | 12 | 4 |
| Rôdeur | 95 | 10 | 4 |
| Clerc | 95 | 12 | 5 |
| Paladin | 105 | 15 | 6 |
| Berserker | 115 | 20 | 7 |

### Ressources de Départ
- Or : 200
- Nourriture : 60
- Soins : 20
- Équipement : 15

## 💡 Conseils de Jeu

1. **Équilibrez** vos héros - une bonne composition aide
2. **Regardez les classes préférées** - les missions bonus selon les classes
3. **Gardez de la nourriture** - c'est essentiel chaque matin
4. **Laissez reposer** les héros blessés - la fatigue diminue avec le repos
5. **Économisez** pour les événements imprévisibles
6. **Recruter stratégiquement** - les nouveaux héros coûtent cher mais strengthissent la guilde

## 📝 Notes de Développement

- Code nettoyé et commentaires inutiles supprimés
- Architecture MVVM pour la maintenabilité
- Tous les chemins de fichiers relatifs
- Entièrement localisé en français
- Pas de dépendances externes autres que .NET

## 🎨 Auteur

Jeu créé en C# avec WPF. Parfait pour l'apprentissage de la gestion de jeu et de l'architecture logicielle en C#.

---

**Amusez-vous bien en gérant votre guilde!** 🗡️⚔️
"# C-Guild-Game-Project" 
