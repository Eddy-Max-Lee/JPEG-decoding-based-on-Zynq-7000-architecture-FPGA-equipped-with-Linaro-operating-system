#ifndef __FFJPEG_ZIGZAG_H__
#define __FFJPEG_ZIGZAG_H__

#ifdef __cplusplus
extern "C" {
#endif

/* ȫ�ֱ������� */
extern const int ZIGZAG[64];

/* �������� */
void zigzag_encode(int *data);
void zigzag_decode_s(int *data);
void zigzag_decode_h(int *data);
void zigzag_decode_h2(int *data);

#ifdef __cplusplus
}
#endif

#endif
