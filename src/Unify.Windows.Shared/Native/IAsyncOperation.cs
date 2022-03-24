using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Unify.Windows.Shared.Native;
[ComImport]
[Guid("3D8B0590-F691-11d2-8EA9-006097DF5BD4")]

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAsyncOperation
{
    void SetAsyncMode([In] Int32 fDoOpAsync);
    void GetAsyncMode([Out] out Int32 pfIsOpAsync);
    void StartOperation([In] IBindCtx pbcReserved);
    void InOperation([Out] out Int32 pfInAsyncOp);
    void EndOperation([In] Int32 hResult, [In] IBindCtx pbcReserved, [In] UInt32 dwEffects);
}