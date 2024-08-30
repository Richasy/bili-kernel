# 哔哩内核

## 概述

哔哩内核（Bili Kernel）是从 [哔哩助理](https://github.com/Richasy/Bili.Copilot) 中抽离出来的核心模块。

它是一层哔哩哔哩 API 的 .NET 包装器，主要做以下事情：

- 将常用 API 封装成 .NET 异步请求
- 将常见的数据结构（比如视频，用户等）抽象成统一的数据结构
- 处理 HTTP 请求，并进行简单的错误处理
- 组件开发，可以按需取用
- 提供接口便于自定义实现

## 开始

> [!WARNING]
> 目前还没有打成 nuget 包，你可能需要 fork 本仓库，然后以 git submodule 的方式引入到你自己的项目中

TBD. 有时间了再写，具体的代码调用可以参考 [BiliService](https://github.com/Richasy/bili-kernel/blob/main/src/Samples/Bili.Console/BiliService.cs)