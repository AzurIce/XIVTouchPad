{
  description = "FFXIV Dalamud Plugin Development Environment";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs =
    {
      self,
      nixpkgs,
      flake-utils,
    }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        pkgs = import nixpkgs { inherit system; };

        # Provides the dotnet SDK.
        # Ideally matches the version required by Dalamud.NET.Sdk (likely 8, 9, or 10 in 2026).
        dotnetSdk = pkgs.dotnet-sdk_10;
      in
      {
        devShells.default = pkgs.mkShell {
          buildInputs = [
            dotnetSdk
          ];

          shellHook = ''
            echo "Welcome to the Dalamud Plugin development shell!"
            echo "Dotnet SDK version: $(dotnet --version)"

            # Note: If you need to point to a specific Dalamud installation:
            export DALAMUD_HOME="/Users/azurice/.xlcore_cn/dalamud/Hooks/dev"
          '';
        };
      }
    );
}
