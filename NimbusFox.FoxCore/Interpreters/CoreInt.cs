using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Interpreters.Classes;
using NimbusFox.FoxCore.Managers;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;

namespace NimbusFox.FoxCore.Interpreters {
    //internal class CoreInt {
    //    protected string script = "";
    //    internal DirectoryManager Directory;
    //    internal ExceptionManager ExceptionManager;
    //    internal bool Debug = false;

    //    protected void ParseScripts(string ext, DirectoryManager currentDir) {
    //        foreach (var dir in currentDir.Directories.Select(currentDir.FetchDirectory)) {
    //            ParseScripts(ext, dir);
    //        }

    //        foreach (var file in currentDir.Files.Where(f => f.Contains(ext))) {
    //            var wait = true;
    //            currentDir.ReadFile<string>(file, code => {
    //                script += Environment.NewLine + code;
    //                wait = false;
    //            }, true);

    //            while (wait) { }
    //        }
    //    }

    //    public virtual void AddCommand(string command, CommandBuilder obj) {
    //        if (CoreIntMods.Commands.ContainsKey(command)) {
    //            throw new Exception("A command with this command call already exists");
    //        }

    //        CoreIntMods.Commands.Add(command, obj);
    //    } 

    //    public virtual void Dispose() { }
    //    public virtual void GameContextInitializeInit() { }
    //    public virtual void GameContextInitializeBefore() { }
    //    public virtual void GameContextInitializeAfter() { }
    //    public virtual void GameContextDeinitialize() { }
    //    public virtual void GameContextReloadBefore() { }
    //    public virtual void GameContextReloadAfter() { }
    //    public virtual void UniverseUpdateBefore(Universe universe, Timestep step) { }
    //    public virtual void UniverseUpdateAfter() { }
    //    public virtual bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
    //        return true;
    //    }

    //    public virtual bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
    //        return true;
    //    }

    //    public virtual bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
    //        return true;
    //    }

    //    public virtual void ClientContextInitializeInit() { }
    //    public virtual void ClientContextInitializeBefore() { }
    //    public virtual void ClientContextInitializeAfter() { }
    //    public virtual void ClientContextDeinitialize() { }
    //    public virtual void ClientContextReloadBefore() { }
    //    public virtual void ClientContextReloadAfter() { }
    //    public virtual void CleanupOldSession() { }
    //    public virtual bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) {
    //        return true;
    //    }

    //    public virtual bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) {
    //        return true;
    //    }
    //}
}
