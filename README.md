# DDDFrame
DDDFrame是基于领域驱动设计模型构建的一套后端通用web框架，它有个突出特点是每个服务有一个主Service，主Service继承基类方法，基类把通用的增删改查接口完全实现了，如果你的业务不涉及到关联对象的处理，这些接口完全可以使用，另外，所有的提交输入对象使用的接口，所以框架又实现了一套独特的序列化方式，只要前端提交的对象里面包含了ObjectName，后端就会根据这个名字去找对应的类进行反序列化。
