# GitHub Copilot Instructions for Nanosuit (Continued) Mod

## Mod Overview and Purpose
The Nanosuit (Continued) mod is an update and continuation of the original Nanosuit mod by Mezz, Ogam, and Taranchuk. It introduces the Crysis Nanosuit into the RimWorld universe, providing players with an overpowered suit that is highly configurable. The mod requires either the Royalty or Biotech DLC due to its jump logic.

## Key Features and Systems
- **Primary Functions**: 
  - **Power Mode**: Enhances accuracy in both range and melee attacks, causing devastating melee hits.
  - **Armor Mode**: Absorbs all forms of damage (melee, range, explosion).
  - **Speed Mode**: Increases movement speed by a factor of x5.
  - **Cloak Mode**: Makes the wearer invisible.
  - **Symbiosis Mode**: Instantly heals any hediff.

- **Secondary Functions**: 
  - **Jumping**: Allows the wearer to jump/skip instead of walk for both work and combat.
  - **Hack Shield**: Disables any shield without stunning the target.
  - **Jump**: Enables jumping to a specified location.

- **Tertiary Functions**: 
  - **Night Visor**: Increases accuracy and removes the darkness debuff.
  - **Psychic Wave Control**: Protects against psychic phenomena and charges the suit's energy.
  - **Environmental Control**: Provides temperature protection from -200 to 200 degrees.
  - **Rebreather**: Allows breathing in space (compatible with Save Our Ship 2).

## Coding Patterns and Conventions
- **C# Code Organization**: The mod is written in C# and organized into various files and classes. Key classes include `Apparel_Nanosuit`, `HarmonyContainer`, and various mode classes (`ApparelMode`, `ArmorMode`, etc.).
- **XML Configuration**: The mod uses XML files for configuration, allowing users to customize the suit's settings. Key XML files include `Keybinding.xml`, `ThingDefs_Misc\Apparel_Headgear.xml`, and `ThingDefs_Misc\Apparel_Various.xml`.

## XML Integration
- **Keybinding Configuration**: The `Keybinding.xml` file defines keybinding categories and specific keybindings for the suit's modes and functions.
- **Thing Definitions**: The `ThingDefs_Misc` folder contains XML files defining various aspects of the Nanosuit, including its appearance, stats, and recipes.

## Harmony Patching
- **Harmony Patches**: The mod uses Harmony to patch existing RimWorld code to integrate new behaviors and functionalities. Key patches include `GenTemperature_ComfortableTemperatureRange.cs` and others found in the `Patches` directory.
- **HarmonyContainer**: A central class that manages the application of Harmony patches.

## Suggestions for Copilot
- **C# Code Assistance**: For adding new functionalities or modifying existing ones, Copilot can suggest code snippets based on existing classes and methods.
- **XML Configuration**: When adding or modifying XML definitions, Copilot can provide templates and examples based on existing XML files.
- **Harmony Patching**: When creating or modifying Harmony patches, Copilot can suggest patterns and examples based on the existing patches in the mod.

## Additional Resources
- **Discord**: Join the mod's Discord server for more information and to chat with creators and the community.
- **GitHub Repository**: For reporting errors or sharing solutions, please use the GitHub repository.

This setup should help GitHub Copilot provide relevant suggestions for developing and maintaining the Nanosuit (Continued) mod.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

