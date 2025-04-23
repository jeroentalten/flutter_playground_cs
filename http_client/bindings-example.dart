import 'dart:ffi';
import 'dart:io';

import 'package:ffi/ffi.dart';

// Define the C function signatures
typedef MakePostRequestNative = Pointer<Utf8> Function(
    Pointer<Utf8> url,
    Pointer<Utf8> username,
    Pointer<Utf8> message,
    );
typedef MakePostRequestDart = Pointer<Utf8> Function(
    Pointer<Utf8> url,
    Pointer<Utf8> username,
    Pointer<Utf8> message,
    );

typedef FreeStringNative = Void Function(Pointer<Utf8> ptr);
typedef FreeStringDart = void Function(Pointer<Utf8> ptr);

class HttpService {
  static final DynamicLibrary _nativeLib = Platform.isWindows
      ? DynamicLibrary.open('/path/to/your/dll/file/ffitest.dll')
      : Platform.isMacOS
      ? DynamicLibrary.open('/path/to/your/framwork/file/ffitest.framework/ffitest')
      : DynamicLibrary.open(
      '/path/to/your/so/file/http_client.so');

  static final MakePostRequestDart _makePostRequest =
  _nativeLib.lookupFunction<MakePostRequestNative, MakePostRequestDart>(
      'send_message');

  static final FreeStringDart _freeString = _nativeLib
      .lookupFunction<FreeStringNative, FreeStringDart>('free_string');

  static String makePostRequest(String url, String username, String message) {
    final urlPtr = url.toNativeUtf8();
    final usernamePtr = username.toNativeUtf8();
    final messagePtr = message.toNativeUtf8();

    try {
      final resultPtr = _makePostRequest(urlPtr, usernamePtr, messagePtr);
      final result = resultPtr.toDartString();
      _freeString(resultPtr);
      return result;
    } finally {
      calloc.free(urlPtr);
      calloc.free(usernamePtr);
      calloc.free(messagePtr);
    }
  }
}