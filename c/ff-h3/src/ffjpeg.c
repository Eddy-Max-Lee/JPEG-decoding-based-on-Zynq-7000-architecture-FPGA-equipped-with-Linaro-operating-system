// ����ͷ�ļ�
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include "jfif.h"
#include "bmp.h"

#include <time.h>

int main(int argc, char *argv[])
{
    //timer define
    double dur;
    clock_t  start, end;
    //timer define ok

    void *jfif = NULL;
    BMP   bmp  = {0};

    if (argc < 3) {
        printf(
            "jfif test program\n"
            "usage: ffjpeg -d filename decode jpg file to decode.bmp\n"
            "       ffjpeg -e filename encode bmp file to encode.jpg\n"
        );
        return 0;
    }

    //user start timer
    start = clock();
    //user start timer ok

    if (strcmp(argv[1], "-d") == 0) {
        //printf("from ffjpeg.c start to jfif_load\n");
        jfif = jfif_load(argv[2]);
        //printf("from ffjpeg.c jfif_load ok\n");
        //printf("from ffjpeg.c start to jfif_decode\n");
        jfif_decode(jfif, &bmp);
        //printf("from ffjpeg.c jfif_decode ok\n");
        //printf("from ffjpeg.c start to jfif_free\n");
        jfif_free  (jfif);
        //printf("from ffjpeg.c jfif_free ok\n");
        bmp_save(&bmp, "decode.bmp");
        bmp_free(&bmp);
    } else if (strcmp(argv[1], "-e") == 0) {
        bmp_load(&bmp, argv[2]);
        jfif = jfif_encode(&bmp);
        bmp_free(&bmp);
        jfif_save(jfif, "encode.jpg");
        jfif_free(jfif);
    }

    //user stop timer
    end = clock();
    //user stop timer ok

    //user print time duration
    dur = (float)(end - start)/CLOCKS_PER_SEC;
    printf( "The process is done, time take *%f+ sec\n",dur);
  //  printf("Use Time: %f\n", dur);
    //user print time duration ok

    return 0;
}
