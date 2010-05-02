﻿namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types

module Object =

  let properties expr = 
    Expr.field expr "Properties"

  let classId expr = 
    Expr.field expr "ClassId"

  let private buildSet ctx name value target =
    let cache, cacheId, cacheIndex = Runtime.PropertyCache.Create(name)
    Wrap.wrapInBlock target (fun obj -> 
      [
        (Expr.debug (sprintf "Setting property '%s'" name))
        (Expr.ternary
          (Expr.eq (classId obj) cacheId)
          (Utils.Box.assign ctx (Expr.access (properties obj) [cacheIndex]) value.Et)
          (Expr.call cache "UpdateSet" [obj; Utils.Box.wrap value.Et; Context.environmentExpr ctx])
        )
      ]
    )

  let private buildGet ctx name (typ:ClrType option) target next =
    let cache, cacheId, cacheIndex = Runtime.PropertyCache.Create(name)
    Wrap.wrapInBlock target (fun obj ->
      let test   = Expr.eq (classId obj) cacheId
      let cached = (Utils.Box.fieldIfClrType (Expr.access (properties obj) [cacheIndex]) typ)
      let update = (Utils.Box.fieldIfClrType (Expr.call cache "UpdateGet" [obj; Context.environmentExpr ctx]) typ)
      match next with
      | Stub.Done -> 
        [
          (Expr.debug (sprintf "Getting property '%s'" name))
          (Expr.ternary test cached update)
        ]
      | Stub.Half(target) -> 
        [
          (Expr.debug (sprintf "Getting property '%s'" name))
          (Expr.ternary
            (test)
            (Stub.combineExpr (Wrap.static'   cached) next)
            (Stub.combineExpr (Wrap.volatile' update) next)
          )
        ]
      | _ -> failwith "Only Stub.Done and Stub.Half allowed"
    )
    
  let unboundSet ctx name value target =
    match target, value with
    | Value(target), Value(value) -> 
      if Runtime.Utils.Type.isObject target.Type 
        then Stub.expr (buildSet ctx name value target)
        else 
          if Runtime.Utils.Type.isBox target.Type then
            Stub.expr (
              Wrap.wrapInBlock target (fun tmp -> 
                [
                  (Expr.debug (sprintf "Type check for setting property '%s'" name))
                  (Expr.ternary
                    (Utils.Box.typeIsT<Runtime.Object> tmp)
                    (buildSet ctx name value (Wrap.static' (Utils.Box.fieldByClrTypeT<Runtime.Object> tmp))).Et
                    (Expr.void')
                  )
                ]
              )
            )
          else
            failwith "Dynamic-only object set not supported"
    | _ -> failwith "Failed"

  let unboundGet ctx name typ target next =
    match target with
    | Value(target) -> 
      if Runtime.Utils.Type.isObject target.Et.Type 
        then Stub.expr (buildGet ctx name typ target next)
        else 
          if Runtime.Utils.Type.isBox target.Type then
            Stub.expr (
              Wrap.wrapInBlock target (fun obj -> 
                [
                  (Expr.debug (sprintf "Type check for getting property '%s'" name))
                  (Expr.ternary
                    (Utils.Box.typeIsT<Runtime.Object> obj)
                    (buildGet ctx name None (Wrap.static' (Utils.Box.fieldByClrTypeT<Runtime.Object> obj)) Done).Et
                    (Expr.defaultT<Runtime.Box>)
                  )
                ]
              )
            )
          else
            failwith "Dynamic-only object set not supported"
    | _ -> failwith "Failed"

  let getProperty (ctx:Context) target name =
    let unbound = unboundGet ctx name None
    Stub.combine (ctx.Build target) (Stub.third unbound)

  let setProperty ctx name = 
    unboundSet ctx name

  let build (ctx:Context) properties id =
    match properties with
    | Some(_) -> failwith "Objects with auto-properties not supported"
    | None    -> 
      if not (ctx.ObjectCaches.ContainsKey id) then
        ctx.ObjectCaches.Add(id, Expr.constant (Runtime.ObjectCache.New(ctx.Environment.BaseClass)))

      let cache = ctx.ObjectCaches.[id]
      let new' = Expr.newArgsT<Runtime.Object> [(Expr.field cache "Class"); (Expr.field cache "InitSize")]
      let assn = Expr.assign (Expr.field cache "LastCreated") new'

      Stub.expr (Wrap.volatile' (assn))