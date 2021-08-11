# Builder image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
RUN apt-get update && apt-get clean && rm -rf /var/lib/apt/lists/*
ADD ./src/services/ /Build/
WORKDIR /Build/2daSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/Alchemy
RUN dotnet publish -c Release -o out
WORKDIR /Build/Area
RUN dotnet publish -c Release -o out
WORKDIR /Build/Arena
RUN dotnet publish -c Release -o out
WORKDIR /Build/AttackSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/BotSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/ChatSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/CommandSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/Config
RUN dotnet publish -c Release -o out
WORKDIR /Build/Craft
RUN dotnet publish -c Release -o out
WORKDIR /Build/DialogSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/DicePoker
RUN dotnet publish -c Release -o out
WORKDIR /Build/DmSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/EnchantmentBasin
RUN dotnet publish -c Release -o out
WORKDIR /Build/EnforceLegalCharacterSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/ExamineSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/FeatSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/Items
RUN dotnet publish -c Release -o out
WORKDIR /Build/LootSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/ModuleSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/PlaceableSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/PlayerSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/SkillSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/SpellSystem
RUN dotnet publish -c Release -o out
WORKDIR /Build/StoreSystem
RUN dotnet publish -c Release -o out

# Build the final NWN server image
FROM index.docker.io/nwndotnet/anvil:b48cf825
LABEL maintainer="chimaera"
COPY --from=build /Build/2daSystem/out /nwn/anvil/Plugins/2daSystem/
COPY --from=build /Build/Alchemy/out /nwn/anvil/Plugins/Alchemy/
COPY --from=build /Build/Area/out /nwn/anvil/Plugins/Area/
COPY --from=build /Build/Arena/out /nwn/anvil/Plugins/Arena/
COPY --from=build /Build/AttackSystem/out /nwn/anvil/Plugins/AttackSystem/
COPY --from=build /Build/BotSystem/out /nwn/anvil/Plugins/BotSystem/
COPY --from=build /Build/ChatSystem/out /nwn/anvil/Plugins/ChatSystem/
COPY --from=build /Build/CommandSystem/out /nwn/anvil/Plugins/CommandSystem/
COPY --from=build /Build/Config/out /nwn/anvil/Plugins/Config/
COPY --from=build /Build/Craft/out /nwn/anvil/Plugins/Craft/
COPY --from=build /Build/DialogSystem/out /nwn/anvil/Plugins/DialogSystem/
COPY --from=build /Build/DicePoker/out /nwn/anvil/Plugins/DicePoker/
COPY --from=build /Build/DmSystem/out /nwn/anvil/Plugins/DmSystem/
COPY --from=build /Build/EnchantmentBasin/out /nwn/anvil/Plugins/EnchantmentBasin/
COPY --from=build /Build/EnforceLegalCharacterSystem/out /nwn/anvil/Plugins/EnforceLegalCharacterSystem/
COPY --from=build /Build/ExamineSystem/out /nwn/anvil/Plugins/ExamineSystem/
COPY --from=build /Build/FeatSystem/out /nwn/anvil/Plugins/FeatSystem/
COPY --from=build /Build/Items/out /nwn/anvil/Plugins/Items/
COPY --from=build /Build/LootSystem/out /nwn/anvil/Plugins/LootSystem/
COPY --from=build /Build/ModuleSystem/out /nwn/anvil/Plugins/ModuleSystem/
COPY --from=build /Build/PlaceableSystem/out /nwn/anvil/Plugins/PlaceableSystem/
COPY --from=build /Build/PlayerSystem/out /nwn/anvil/Plugins/PlayerSystem/
COPY --from=build /Build/SkillSystem/out /nwn/anvil/Plugins/SkillSystem/
COPY --from=build /Build/SpellSystem/out /nwn/anvil/Plugins/SpellSystem/
COPY --from=build /Build/StoreSystem/out /nwn/anvil/Plugins/StoreSystem/